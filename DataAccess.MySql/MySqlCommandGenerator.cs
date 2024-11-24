using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Data;
using DataAccess.Core.Data;
using System.Collections;
using MySql.Data.MySqlClient;
using DataAccess.Core.Interfaces;
using System.Data.Common;

namespace DataAccess.MySql
{
    /// <summary>
    /// Generates various types of data store Commands, appropriate for mysql
    /// </summary>
    public class MySqlCommandGenerator : DatabaseCommandGenerator
    {
        /// <summary>
        /// {0} = FK table
        /// {1} = PK Table
        /// {2} = PK Table Column
        /// {3} = Constraint Type
        /// {4} = FK Column
        /// </summary>
        private static string _createFKSQL = ",CONSTRAINT `FK_{0}_{1}` FOREIGN KEY `FK_{0}_{1}` ({4}) REFERENCES {1} ({2}) ON DELETE {3} ON UPDATE {3}";

        /// <summary>
        /// {0}  = FK Table
        /// {1} = PK Table
        /// {2} = FK Column
        /// {3} = PK table Column
        /// {4} = Constraint Type
        /// </summary>
        private static string _addFKSql = "ADD CONSTRAINT `FK_{0}_{1}` FOREIGN KEY `FK_{0}_{1}` ({2}) REFERENCES {1} ({3}) ON DELETE {4} ON UPDATE {4};";

        /// <summary>
        /// {0} = Table
        /// {1} = Column
        /// {2} = New Type
        /// </summary>
        private static string _ModifyColumn = "ALTER TABLE {0} MODIFY COLUMN {1} {2};";

        private StorageEngine StorageEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlCommandGenerator"/> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public MySqlCommandGenerator(StorageEngine engine, IDataConnection conn)
            : base(conn)
        {
            this.StorageEngine = engine;
        }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        public override DbCommand GetInsertCommand(object item)
        {
            IEnumerable<DataFieldInfo> info = TypeParser.GetPrimaryKeys(item.GetType());
            DbCommand cmd = base.GetInsertCommand(item);
            if (info.Count() == 1 && !info.ElementAt(0).SetOnInsert)
                cmd.CommandText = cmd.CommandText.Replace(";", string.Format("; SELECT LAST_INSERT_ID() as {0};", info.First().FieldName));
            return cmd;
        }

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public override IEnumerable<DbCommand> GetAddTableCommand(DatabaseTypeInfo ti)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            StringBuilder contrain = new StringBuilder();
            MySqlCommand cmd = new MySqlCommand();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti, false));
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(",");

                if (dfi.PrimaryKey)
                {
                    if (pFields.Length > 0) pFields.Append(",");
                    pFields.AppendFormat("{0}", dfi.FieldName);

                    if (dfi.PropertyType == typeof(int) && ti.PrimaryKeys.Count == 1)
                        sb.AppendFormat("{0} {1} NOT NULL AUTO_INCREMENT ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                    else
                        sb.AppendFormat("{0} {1} NOT NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                }
                else
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                if (dfi.PrimaryKeyType != null && StorageEngine == MySql.StorageEngine.InnoDB)
                {
                    DatabaseTypeInfo pkType = TypeParser.GetTypeInfo(dfi.PrimaryKeyType);
                    contrain.AppendFormat(_createFKSQL, ResolveTableName(ti, false), pkType.UnescapedTableName, pkType.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(dfi.ForeignKeyType), dfi.EscapedFieldName);
                }
            }

            if (pFields.Length > 0)
                sb.AppendFormat(",PRIMARY KEY ({0})", pFields.ToString());

            sb.Append(contrain.ToString());
            sb.AppendFormat(") ENGINE = {0}", StorageEngine.ToString());

            cmd.CommandText = sb.ToString();
            yield return cmd;
        }

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public override DbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            MySqlCommand command = new MySqlCommand();
            command.CommandText = $"ALTER TABLE `{ResolveTableName(type, false)}` DROP COLUMN `{dfi.FieldName}`;";
            return command;
        }

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public override IEnumerable<DbCommand> GetAddColumnCommnad(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                MySqlCommand scmd = new MySqlCommand();
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ALTER TABLE {0} ADD {1} {2} NULL", ResolveTableName(type, false), dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                if (dfi.PrimaryKeyType != null)
                {
                    DatabaseTypeInfo pkType = TypeParser.GetTypeInfo(dfi.PrimaryKeyType);
                    sb.Append(',');
                    sb.AppendFormat(_addFKSql, ResolveTableName(type, false), ResolveTableName(pkType, false), dfi.EscapedFieldName, pkType.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(dfi.ForeignKeyType));
                }

                sb.Append(';');
                scmd.CommandText = sb.ToString();
                yield return scmd;
            }
        }

        /// <summary>
        /// Returns a command appropriate for modifying a column in MYSQL
        /// </summary>
        /// <param name="type">The type (Table info)</param>
        /// <param name="dfi">The field to edit</param>
        /// <param name="targetFieldType">the new column type</param>
        /// <returns></returns>
        public override IEnumerable<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = string.Format(_ModifyColumn, ResolveTableName(type, false), dfi.EscapedFieldName, targetFieldType);
            yield return cmd;
        }        

        /// <summary>
        /// Returns null (mysql doesnt support this)
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public override DbCommand GetAddSchemaCommand(DatabaseTypeInfo ti)
        {
            return null;
        }
    }
}
