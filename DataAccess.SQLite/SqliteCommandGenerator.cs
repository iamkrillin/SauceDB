using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using System.Data;
using DataAccess.Core.Data;
using System.Data.SQLite;

namespace DataAccess.SQLite
{
    /// <summary>
    /// Generates various types of data store Commands, appropriate for SQLite
    /// </summary>
    public class SqliteCommandGenerator : DatabaseCommandGenerator
    {
        /// <summary>
        /// Returns the name of a column
        /// </summary>
        /// <param name="PropertyName">The objects property to use</param>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public override string ResolveFieldName(string PropertyName, Type type)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(type);
            if (ti.IsView)
                return ti.DataFields.Where(r => r.PropertyName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().FieldName;
            else
                return base.ResolveFieldName(PropertyName, type);
        }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        public override IDbCommand GetInsertCommand(object item)
        {
            IEnumerable<DataFieldInfo> info = DataStore.TypeInformationParser.GetPrimaryKeys(item.GetType());
            IDbCommand cmd = base.GetInsertCommand(item);
            if (info.Count() == 1)
                cmd.CommandText = cmd.CommandText.Replace(";", string.Format("; SELECT last_insert_rowid() as {0};", info.First().EscapedFieldName));

            return cmd;
        }

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddTableCommand(DatabaseTypeInfo ti)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder pFields = new StringBuilder();
            SQLiteCommand cmd = new SQLiteCommand();
            StringBuilder fKey = new StringBuilder();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti, false));
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(",");

                if (dfi.PrimaryKey)
                {
                    sb.AppendFormat("{0} {1}", dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                    if (dfi.PropertyType == typeof(int))
                    {
                        sb.Append(" PRIMARY KEY AUTOINCREMENT");
                    }
                    else
                    {
                        if (pFields.Length > 0) pFields.Append(",");
                        pFields.Append(dfi.EscapedFieldName);
                    }
                }
                else
                {
                    sb.AppendFormat("{0} {1}", dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                    if (dfi.PrimaryKeyType == null)
                        sb.Append(" NULL");

                    if (!string.IsNullOrEmpty(dfi.DefaultValue)) sb.AppendFormat(" DEFAULT {0}", dfi.DefaultValue);
                }

                if (dfi.PrimaryKeyType != null)
                {
                    if (fKey.Length > 0) fKey.Append(",");
                    fKey.Append(GetForeignKeySql(dfi, ti, DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType)));
                }
            }

            if (fKey.Length > 0)
                sb.AppendFormat(", {0}", fKey.ToString());

            if (pFields.Length > 0)
                sb.AppendFormat(", PRIMARY KEY({0})", pFields);

            sb.Append(");");
            cmd.CommandText = sb.ToString();
            yield return cmd;
        }

        private string GetForeignKeySql(DataFieldInfo field, DatabaseTypeInfo targetTable, DatabaseTypeInfo pkeyTable)
        {
            return string.Format(" FOREIGN KEY({0}) REFERENCES {1}({2}) ON DELETE {3} ON UPDATE {3}", field.EscapedFieldName, ResolveTableName(pkeyTable, false), pkeyTable.PrimaryKeys.First().EscapedFieldName, TranslateFkeyType(field.ForeignKeyType));
        }

        /// <summary>
        /// Returns a command for removing a column from a table (Not supported for sqlite)
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public override IDbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            throw new DataStoreException("This is not supported by SQLite");
        }

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetAddColumnCommnad(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            if (dfi.PrimaryKey)
                throw new DataStoreException("Adding a primary key to an existing table is not supported");
            else
            {
                SQLiteCommand scmd = new SQLiteCommand();
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("ALTER TABLE {0} ADD COLUMN {1} {2}", ResolveTableName(type, false), dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                if (!string.IsNullOrEmpty(dfi.DefaultValue))
                    sb.AppendFormat(" DEFAULT {0}", dfi.DefaultValue);

                scmd.CommandText = sb.ToString();
                yield return scmd;
            }
        }

        /// <summary>
        /// Returns a command for modifying a column to the specified type (Not supported for sqlite)
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <param name="targetFieldType">The type to change the field to</param>
        /// <returns></returns>
        public override IEnumerable<IDbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            throw new Exception("this is not supported right now");
            /*
            List<IDbCommand> toReturn = new List<IDbCommand>();
            string newCol = Guid.NewGuid().ToString().Replace("-", "");
            //EscapedFieldName, DataFieldType, 

            toReturn.AddRange(GetAddColumnCommnad(type, new DataFieldInfo() { DataFieldType = dfi.DataFieldType, EscapedFieldName = newCol, PropertyType = dfi.PropertyType }));
            toReturn.Add(new SQLiteCommand(string.Format("update {0} set {1} = {2}", ResolveTableName(type, true), newCol, dfi.EscapedFieldName)));
            //toReturn.Add(GetRemoveColumnCommand(type, dfi));
            toReturn.AddRange(GetAddColumnCommnad(type, dfi));
            toReturn.Add(new SQLiteCommand(string.Format("update {0} set {2} = {1}", ResolveTableName(type, true), newCol, dfi.EscapedFieldName)));
        
            return toReturn;
             * */
        }

        /// <summary>
        /// Returns null (sqlite doesnt support this)
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public override IDbCommand GetAddSchemaCommand(DatabaseTypeInfo ti)
        {
            return null;
        }
    }
}
