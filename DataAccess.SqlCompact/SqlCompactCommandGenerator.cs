using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Data;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Data.SqlServerCe;

namespace DataAccess.SqlCompact
{
    /// <summary>
    /// Generates various types of data store Commands, appropriate for Sql Server
    /// </summary>
    public class SqlCompactCommandGenerator : DatabaseCommandGenerator
    {
        static SqlCompactCommandGenerator()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCompactCommandGenerator"/> class.
        /// </summary>
        public SqlCompactCommandGenerator()
        {
        }

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddTableCommand(TypeInfo ti)
        {
            List<IDbCommand> toReturn = new List<IDbCommand>();
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            SqlCeCommand cmd = new SqlCeCommand();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti));
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

                    if (dfi.PropertyType == typeof(string) && string.IsNullOrEmpty(dfi.DefaultValue)) dfi.DefaultValue = "newid()"; //if the user specifies a default value, dont override it
                }
                else
                {
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                }

                if (dfi.PrimaryKeyType != null)
                {
                    SqlCeCommand fk = new SqlCeCommand();
                    fk.CommandText = GetForeignKeySql(dfi, ti, DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType));
                    toReturn.Add(fk);
                }

                if (!string.IsNullOrEmpty(dfi.DefaultValue))
                {
                    SqlCeCommand dv = new SqlCeCommand();
                    dv.CommandText = GetDefaultValueSql(ti, dfi, dv);
                    toReturn.Add(dv);
                }
            }
            sb.Append(");");

            if (pFields.Length > 0)
            {
                SqlCeCommand pKey = new SqlCeCommand();
                pKey.CommandText = string.Format("ALTER TABLE {0} ADD CONSTRAINT PK_{2}_{3} PRIMARY KEY ({1});",
                                ResolveTableName(ti), pFields.ToString(), ti.UnEscapedSchema, ti.UnescapedTableName);

                toReturn.Insert(0, pKey);
            }

            cmd.CommandText = sb.ToString();
            toReturn.Insert(0, cmd);
            return toReturn;
        }

        private string GetDefaultValueSql(TypeInfo ti, DataFieldInfo dfi, SqlCeCommand cmd)
        {
            return string.Format("ALTER TABLE {0} ADD CONSTRAINT DF_{3}_{4}_{5} DEFAULT {2} FOR {1};", ResolveTableName(ti), dfi.FieldName, dfi.DefaultValue, ti.UnEscapedSchema, ti.UnescapedTableName, dfi.FieldName);
        }

        private string GetForeignKeySql(DataFieldInfo field, TypeInfo targetTable, TypeInfo pKeyTable)
        {
            StringBuilder sb = new StringBuilder("ALTER TABLE ");
            sb.AppendFormat("{0} ", ResolveTableName(targetTable));
            sb.AppendFormat("ADD CONSTRAINT FK_{0}_{1}_{2} ", targetTable.UnEscapedSchema, targetTable.UnescapedTableName, field.FieldName);
            sb.AppendFormat("FOREIGN KEY({0}) ", field.EscapedFieldName);
            sb.AppendFormat("REFERENCES {0} ({1}) ",ResolveTableName(pKeyTable), pKeyTable.PrimaryKeys[0].EscapedFieldName);
            sb.AppendFormat("ON UPDATE {0} ON DELETE {0};", TranslateFkeyType(field.ForeignKeyType));

            return sb.ToString();
        }

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public override IDbCommand GetRemoveColumnCommand(TypeInfo type, DataFieldInfo dfi)
        {
            SqlCeCommand scmd = new SqlCeCommand();
            scmd.CommandText = string.Format("ALTER TABLE {0} DROP COLUMN [{1}]", ResolveTableName(type), dfi.FieldName);
            return scmd;
        }

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddColumnCommnad(TypeInfo type, DataFieldInfo dfi)
        {
            List<IDbCommand> toReturn = new List<IDbCommand>();

            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                SqlCeCommand scmd = new SqlCeCommand();
                scmd.CommandText = string.Format("ALTER TABLE {0} ADD {1} {2} NULL;", ResolveTableName(type), dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                toReturn.Add(scmd);

                if (dfi.PrimaryKeyType != null)
                {
                    SqlCeCommand fk = new SqlCeCommand();
                    fk.CommandText = GetForeignKeySql(dfi, type, DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType));
                    toReturn.Add(fk);
                }

                if (!string.IsNullOrEmpty(dfi.DefaultValue))
                {
                    SqlCeCommand dv = new SqlCeCommand();
                    dv.CommandText = GetDefaultValueSql(type, dfi, dv);
                    toReturn.Add(dv);
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
        public override IEnumerable<IDbCommand> GetModifyColumnCommand(TypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            List<IDbCommand> toReturn = new List<IDbCommand>();
            SqlCeCommand cmd = new SqlCeCommand();
            cmd.CommandText = string.Format("ALTER TABLE {0} ALTER COLUMN [{1}] {2}", ResolveTableName(type), dfi.FieldName, targetFieldType);
            toReturn.Add(cmd);


            if (dfi.PrimaryKeyType != null)
            {
                SqlCeCommand fk = new SqlCeCommand();
                fk.CommandText = GetForeignKeySql(dfi, type, DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType));
                toReturn.Add(fk);
            }

            if (!string.IsNullOrEmpty(dfi.DefaultValue))
            {
                SqlCeCommand dv = new SqlCeCommand();
                dv.CommandText = GetDefaultValueSql(type, dfi, dv);
                toReturn.Add(dv);
            }

            return toReturn;
        }

        /// <summary>
        /// /// Gets a table name for a type
        /// </summary>
        /// <param name="ti">The table name</param>
        /// <param name="EscapeTableName">ignored</param>
        /// <returns></returns>
        public override string ResolveTableName(TypeInfo ti, bool EscapeTableName)
        {
            return ResolveTableName(ti);
        }

        /// <summary>
        /// Gets a table name for a type
        /// </summary>
        /// <param name="ti">The type</param>
        /// <returns></returns>
        public override string ResolveTableName(TypeInfo ti)
        {
            if (!string.IsNullOrEmpty(ti.UnEscapedSchema))
                return string.Concat(ti.UnEscapedSchema, "_", ti.UnescapedTableName);
            else
                return ti.TableName;
        }

        public override string ResolveTableName(string schema, string table)
        {
            if (!string.IsNullOrEmpty(schema))
                return string.Concat(schema, "_", table);
            else
                return table;
        }

        public override IDbCommand GetAddSchemaCommand(TypeInfo ti)
        {
            return null;
        }
    }
}
