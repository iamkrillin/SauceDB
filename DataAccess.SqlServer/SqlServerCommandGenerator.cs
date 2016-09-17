using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Data;
using DataAccess.Core.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;

namespace DataAccess.SqlServer
{
    /// <summary>
    /// Generates various types of data store Commands, appropriate for Sql Server
    /// </summary>
    public class SqlServerCommandGenerator : DatabaseCommandGenerator
    {
        private static string _createSchema;

        static SqlServerCommandGenerator()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            _createSchema = asmb.LoadResource("DataAccess.SqlServer.Sql.CreateSchema.sql");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCommandGenerator"/> class.
        /// </summary>
        public SqlServerCommandGenerator()
        {
        }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        public override IDbCommand GetInsertCommand(object item)
        {
            DatabaseTypeInfo ti = base.DataStore.TypeInformationParser.GetTypeInfo(item.GetType());
            IDbCommand cmd = base.GetInsertCommand(item);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public override IDbCommand GetInsertCommand(IList items)
        {//this can cause issues with triggers.
            IDbCommand cmd = base.GetInsertCommand(items);
            cmd.CommandText = cmd.CommandText.Replace("VALUES", "output inserted.* VALUES");
            return cmd;
        }

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddTableCommand(DatabaseTypeInfo ti)
        {
            List<IDbCommand> toReturn = new List<IDbCommand>();
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            SqlCommand cmd = new SqlCommand();

            sb.AppendFormat("CREATE TABLE {0}.{1} (", ti.Schema, ti.TableName);
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(",");

                if (dfi.PrimaryKey)
                {
                    if (pFields.Length > 0) pFields.Append(",");
                    pFields.Append(dfi.FieldName);

                    if (dfi.PropertyType == typeof(int) && ti.PrimaryKeys.Count == 1)
                        sb.AppendFormat("{0} {1} NOT NULL IDENTITY(1,1) ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                    else
                        sb.AppendFormat("{0} {1} NOT NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                }
                else
                {
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                }

                if (dfi.PrimaryKeyType != null)
                {
                    SqlCommand fk = new SqlCommand();
                    fk.CommandText = GetForeignKeySql(dfi, ti, DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType));
                    toReturn.Add(fk);
                }
            }
            sb.Append(") ON [PRIMARY];");

            if (pFields.Length > 0)
            {
                SqlCommand pKey = new SqlCommand();
                pKey.CommandText = string.Format("ALTER TABLE {0}.{1} ADD CONSTRAINT PK_{3}_{4} PRIMARY KEY CLUSTERED ({2})",
                                ti.Schema, ti.TableName, pFields.ToString(), ti.UnEscapedSchema, ti.UnescapedTableName);

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

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public override IDbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            SqlCommand scmd = new SqlCommand();
            scmd.CommandText = string.Format("ALTER TABLE {0}.{1} DROP COLUMN [{2}]", type.Schema, type.TableName, dfi.FieldName);
            return scmd;
        }

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddColumnCommnad(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            List<IDbCommand> toReturn = new List<IDbCommand>();

            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                SqlCommand scmd = new SqlCommand();
                scmd.CommandText = string.Format("ALTER TABLE {0} ADD {1} {2} NULL;", ResolveTableName(type), dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                toReturn.Add(scmd);

                if (dfi.PrimaryKeyType != null)
                {
                    SqlCommand fk = new SqlCommand();
                    fk.CommandText = GetForeignKeySql(dfi, type, DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType));
                    toReturn.Add(fk);
                }
            }

            return toReturn;
        }        

        /// <summary>
        /// Gets a command for changing a column type
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="dfi">The field</param>
        /// <param name="targetFieldType">The new column type</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = string.Format("ALTER TABLE {0} ALTER COLUMN [{1}] {2}", ResolveTableName(type), dfi.FieldName, targetFieldType);
            yield return cmd;

        }

        /// <summary>
        /// /// Gets a table name for a type
        /// </summary>
        /// <param name="ti">The table name</param>
        /// <param name="EscapeTableName">ignored</param>
        /// <returns></returns>
        public override string ResolveTableName(DatabaseTypeInfo ti, bool EscapeTableName)
        {
            return ResolveTableName(ti);
        }

        /// <summary>
        /// Gets a table name for a type
        /// </summary>
        /// <param name="ti">The type</param>
        /// <returns></returns>
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

        public override IDbCommand GetAddSchemaCommand(DatabaseTypeInfo ti)
        {
            if (!ti.UnEscapedSchema.Equals("dbo", StringComparison.InvariantCultureIgnoreCase))
            {
                IDbCommand cmd = new SqlCommand();
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
