using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Core;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Data;
using System.Collections;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.SqlServer
{
    /// <summary>
    /// Generates various types of data store Commands, appropriate for Sql Server
    /// </summary>
    public class SqlServerCommandGenerator(IDataConnection connection) : DatabaseCommandGenerator(connection)
    {
        private static string _createSchema;

        static SqlServerCommandGenerator()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            _createSchema = asmb.LoadResource("DataAccess.SqlServer.Sql.CreateSchema.sql");
        }

        public override async Task<DbCommand> GetInsertCommand(TypeParser tparser, object item)
        {
            DatabaseTypeInfo ti = await tparser.GetTypeInfo(item.GetType());
            DbCommand cmd = await base.GetInsertCommand(tparser, item);

            if (!ti.IsCompilerGenerated)
            {
                if (ti.PrimaryKeys.Count == 1 && !ti.PrimaryKeys[0].SetOnInsert)
                {
                    if (ti.PrimaryKeys[0].PropertyType == typeof(string))
                    {
                        cmd.CommandText = cmd.CommandText.Replace("VALUES", "output inserted.* VALUES");
                    }
                    else
                    {//this will only work with int keys
                        cmd.CommandText = cmd.CommandText.Replace(";", "; select SCOPE_IDENTITY() as " + ti.PrimaryKeys[0].EscapedFieldName);
                    }
                }
            }

            return cmd;
        }

        public override async Task<DbCommand> GetInsertCommand(TypeParser tparser, IList items)
        {//this can cause issues with triggers.
            DbCommand cmd = await base.GetInsertCommand(tparser, items);
            cmd.CommandText = cmd.CommandText.Replace("VALUES", "output inserted.* VALUES");
            return cmd;
        }

        public override async Task<List<DbCommand>> GetAddTableCommand(TypeParser tparser, DatabaseTypeInfo ti)
        {
            List<DbCommand> toReturn = [];
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            SqlCommand cmd = new SqlCommand();

            sb.AppendFormat("CREATE TABLE {0}.{1} (", ti.Schema, ti.TableName);
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(',');

                if (dfi.PrimaryKey)
                {
                    if (pFields.Length > 0) pFields.Append(',');
                    pFields.Append(dfi.FieldName);

                    if (dfi.PropertyType == typeof(int) && ti.PrimaryKeys.Count == 1)
                        sb.AppendFormat("{0} {1} NOT NULL IDENTITY(1,1) ", dfi.EscapedFieldName, await TranslateTypeToSql(tparser, dfi));
                    else
                        sb.AppendFormat("{0} {1} NOT NULL ", dfi.EscapedFieldName, await TranslateTypeToSql(tparser, dfi));
                }
                else
                {
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, await TranslateTypeToSql(tparser, dfi));
                }

                if (dfi.PrimaryKeyType != null)
                {
                    SqlCommand fk = new SqlCommand();
                    fk.CommandText = GetForeignKeySql(dfi, ti, await tparser.GetTypeInfo(dfi.PrimaryKeyType));
                    toReturn.Add(fk);
                }
            }
            sb.Append(") ON [PRIMARY];");

            if (pFields.Length > 0)
            {
                SqlCommand pKey = new SqlCommand();
                pKey.CommandText = $"ALTER TABLE {ti.Schema}.{ti.TableName} ADD CONSTRAINT PK_{ti.UnEscapedSchema}_{ti.UnescapedTableName} PRIMARY KEY CLUSTERED ({pFields})";
                toReturn.Insert(0, pKey);
            }

            cmd.CommandText = sb.ToString();
            toReturn.Insert(0, cmd);
            return toReturn;
        }

        private string GetForeignKeySql(DataFieldInfo field, DatabaseTypeInfo targetTable, DatabaseTypeInfo pKeyTable)
        {
            StringBuilder sb = new StringBuilder("ALTER TABLE ");
            sb.AppendFormat("{0}.{1} ", targetTable.Schema, targetTable.TableName);
            sb.AppendFormat("ADD CONSTRAINT FK_{0}_{1}_{2} ", targetTable.UnEscapedSchema, targetTable.UnescapedTableName, field.FieldName);
            sb.AppendFormat("FOREIGN KEY({0}) ", field.EscapedFieldName);
            sb.AppendFormat("REFERENCES {0}.{1} ({2}) ", pKeyTable.Schema, pKeyTable.TableName, pKeyTable.PrimaryKeys[0].EscapedFieldName);
            sb.AppendFormat("ON UPDATE {0} ON DELETE {0};", TranslateFkeyType(field.ForeignKeyType));

            return sb.ToString();
        }

        public override DbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            SqlCommand scmd = new SqlCommand();
            scmd.CommandText = $"ALTER TABLE {type.Schema}.{type.TableName} DROP COLUMN [{dfi.FieldName}]";
            return scmd;
        }

        public override async Task<List<DbCommand>> GetAddColumnCommnad(TypeParser tparser, DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            List<DbCommand> toReturn = [];

            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                SqlCommand scmd = new SqlCommand();
                scmd.CommandText = $"ALTER TABLE {ResolveTableName(type)} ADD {dfi.EscapedFieldName} {await TranslateTypeToSql(tparser, dfi)} NULL;";
                toReturn.Add(scmd);

                if (dfi.PrimaryKeyType != null)
                {
                    SqlCommand fk = new SqlCommand();
                    fk.CommandText = GetForeignKeySql(dfi, type, await tparser.GetTypeInfo(dfi.PrimaryKeyType));
                    toReturn.Add(fk);
                }
            }

            return toReturn;
        }        

        public override List<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = $"ALTER TABLE {ResolveTableName(type)} ALTER COLUMN [{dfi.FieldName}] {targetFieldType}";
            return [cmd];
        }

        public override string ResolveTableName(DatabaseTypeInfo ti, bool EscapeTableName)
        {
            return ResolveTableName(ti);
        }

        public override string ResolveTableName(DatabaseTypeInfo ti)
        {
            if (!string.IsNullOrEmpty(ti.Schema))
                return string.Concat(ti.Schema, ".", ti.TableName);
            else
                return ti.TableName;
        }

        public override string ResolveTableName(string schema, string table)
        {
            if (!string.IsNullOrEmpty(schema))
                return string.Concat(schema, ".", table);
            else
                return table;
        }

        public override DbCommand GetAddSchemaCommand(DatabaseTypeInfo ti)
        {
            if (!ti.UnEscapedSchema.Equals("dbo", StringComparison.InvariantCultureIgnoreCase))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format(_createSchema, ti.UnEscapedSchema);
                return cmd;
            }
            else
            {
                return null;
            }
        }
    }
}
