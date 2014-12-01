using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Data;
using DataAccess.Core.Data;
using Npgsql;
using System.Reflection;

namespace DataAccess.PostgreSQL
{
    /// <summary>
    /// Command generator for postgre
    /// </summary>
    public class PostgreCommandGenerator : DatabaseCommandGenerator
    {
        private static string _createFKSQL = @"CONSTRAINT ""{0}"" FOREIGN KEY ({1}) REFERENCES {2} ({3}) MATCH SIMPLE ON UPDATE {4} ON DELETE {4}";
        private static string _addFKSql = @"ADD CONSTRAINT ""{0}"" FOREIGN KEY ({1}) REFERENCES {2} ({3}) MATCH SIMPLE ON UPDATE {4} ON DELETE {4}";
        private static string _ModifyColumn = "ALTER TABLE {0} ALTER COLUMN {1} TYPE {2};";

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddTableCommand(TypeInfo ti)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            StringBuilder contrain = new StringBuilder();
            NpgsqlCommand cmd = new NpgsqlCommand();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti, false));
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(",");

                if (dfi.PrimaryKey)
                {
                    if (pFields.Length > 0) pFields.Append(",");
                    pFields.AppendFormat("{0}", dfi.EscapedFieldName);

                    if (dfi.PropertyType == typeof(int) && ti.PrimaryKeys.Count == 1)
                        sb.AppendFormat("{0} serial NOT NULL ", dfi.EscapedFieldName);
                    else
                        sb.AppendFormat("{0} {1} NOT NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                }
                else
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                if (dfi.PrimaryKeyType != null)
                {
                    if (contrain.Length > 0) contrain.Append(",");
                    TypeInfo pkType = DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType);
                    contrain.AppendFormat(_createFKSQL, Guid.NewGuid().ToString(), dfi.EscapedFieldName, ResolveTableName(pkType, false), pkType.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(dfi.ForeignKeyType));
                }

                if (!string.IsNullOrEmpty(dfi.DefaultValue))
                {
                    if (dfi.PropertyType == typeof(int))
                        sb.AppendFormat("DEFAULT {0}", dfi.DefaultValue);
                    else
                        sb.AppendFormat("DEFAULT {0}", dfi.DefaultValue);
                }
            }

            if (pFields.Length > 0)
                sb.AppendFormat(", CONSTRAINT \"{0}\" PRIMARY KEY ({1})", Guid.NewGuid().ToString(), pFields.ToString());

            if (contrain.Length > 0)
            {
                if (pFields.Length > 0) sb.Append(",");
                sb.Append(contrain.ToString());
            }

            sb.Append(");");
            cmd.CommandText = sb.ToString();
            yield return cmd;
        }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        public override IDbCommand GetInsertCommand(object item)
        {
            IDbCommand cmd = base.GetInsertCommand(item);
            cmd.CommandText = cmd.CommandText.Replace(";", " returning *;");
            return cmd;
        }

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public override IDbCommand GetRemoveColumnCommand(TypeInfo type, DataFieldInfo dfi)
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.CommandText = string.Format("ALTER TABLE {0} DROP COLUMN {1};", ResolveTableName(type, false), dfi.FieldName);
            return command;
        }

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddColumnCommnad(TypeInfo type, DataFieldInfo dfi)
        {
            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                NpgsqlCommand scmd = new NpgsqlCommand();
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ALTER TABLE {0} ADD {1} {2} NULL ", ResolveTableName(type, false), dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                if (!string.IsNullOrEmpty(dfi.DefaultValue))
                {
                    if (dfi.PropertyType == typeof(int))
                        sb.AppendFormat("DEFAULT {0}", dfi.DefaultValue);
                    else
                        sb.AppendFormat("DEFAULT {0}", dfi.DefaultValue);
                }

                if (dfi.PrimaryKeyType != null)
                {
                    TypeInfo pkType = DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType);
                    sb.Append(",");
                    sb.AppendFormat(_addFKSql, Guid.NewGuid().ToString(), dfi.EscapedFieldName, ResolveTableName(pkType, false), pkType.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(dfi.ForeignKeyType));
                }

                sb.Append(";");
                scmd.CommandText = sb.ToString();
                yield return scmd;
            }
        }

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <param name="targetFieldType">The type to change the field to</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetModifyColumnCommand(TypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            targetFieldType = targetFieldType.Replace("varchar", "character varying");
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.CommandText = string.Format(_ModifyColumn, ResolveTableName(type, false), dfi.EscapedFieldName, targetFieldType);
            yield return cmd;
        }

        /// <summary>
        /// Returns the name of the table (schema.table)
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public override string ResolveTableName(Type type)
        {
            return ResolveTableName(DataStore.TypeInformationParser.GetTypeInfo(type), false);
        }

        /// <summary>
        /// Resolves the name of the table.
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <returns></returns>
        public override string ResolveTableName(TypeInfo ti)
        {
            return ResolveTableName(ti, false);
        }

        /// <summary>
        /// Returns null (postgre doesnt support this)
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public override IDbCommand GetAddSchemaCommand(TypeInfo ti)
        {
            return null;
        }
    }
}
