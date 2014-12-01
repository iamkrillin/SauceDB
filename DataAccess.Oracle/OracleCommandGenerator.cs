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
using System.Data.OracleClient;

namespace DataAccess.Oracle
{
    /// <summary>
    /// Generates various types of data store Commands, appropriate for Sql Server
    /// </summary>
    public class OracleCommandGenerator : DatabaseCommandGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleCommandGenerator"/> class.
        /// </summary>
        public OracleCommandGenerator()
        {
        }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        public override IDbCommand GetInsertCommand(object item)
        {
            TypeInfo ti = base.DataStore.TypeInformationParser.GetTypeInfo(item.GetType());
            if (ti.PrimaryKeys.Count == 1)
            {
                IDbCommand cmd = base.GetInsertCommand(item);
                cmd.CommandText = cmd.CommandText.Replace(";", "; select SCOPE_IDENTITY() as " + ti.PrimaryKeys[0].FieldName);
                return cmd;
            }
            else
            {
                return base.GetInsertCommand(item);
            }
        }

        protected override List<ParameterData> GetObjectParameters(int startingIndex, object item, TypeInfo TypeInfo)
        {
            List<ParameterData> toReturn = new List<ParameterData>();
            foreach (DataFieldInfo info in TypeInfo.DataFields)
            {
                if (info.SetOnInsert)
                {
                    object value = info.Getter(item, null);
                    if (value != null)
                    {
                        value = ConvertToOracle(value, info.PropertyType);

                        IDbDataParameter parm = DataStore.Connection.GetParameter();
                        parm.Value = value;
                        parm.ParameterName = (++startingIndex).ToString();
                        toReturn.Add(new ParameterData(parm, info.EscapedFieldName));
                    }
                }
            }

            return toReturn;
        }

        private object ConvertToOracle(object value, Type t)
        {
            if (t == typeof(TimeSpan))
            {
                return new System.Data.OracleClient.OracleTimeSpan((TimeSpan)value);
            }
            else
            {
                return DataStore.Connection.TypeConverter.ConvertToType(value, t);
            }
        }

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public override IDbCommand GetAddTableCommand(TypeInfo ti)
        {
            StringBuilder sb = new StringBuilder();
            OracleCommand cmd = new OracleCommand();

            sb.AppendFormat("CREATE TABLE {0} (", ResolveTableName(ti, false));
            for (int i = 0; i < ti.DataFields.Count; i++)
            {
                DataFieldInfo dfi = ti.DataFields[i];
                if (i > 0) sb.Append(",");

                if (dfi.PrimaryKey)
                {
                    if (dfi.PropertyType == typeof(int) && ti.PrimaryKeys.Count == 1)
                        sb.AppendFormat("{0} {1} PRIMARY KEY ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                    else
                        sb.AppendFormat("{0} {1} NOT NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));
                }
                else
                    sb.AppendFormat("{0} {1} NULL ", dfi.EscapedFieldName, TranslateTypeToSql(dfi));

                if (dfi.PrimaryKeyType != null)
                {
                    TypeInfo pkType = DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType);
                    sb.AppendFormat("CONSTRAINT fk_{0}_{1}_{2} REFERENCES {1} ({2}))", ti.UnescapedTableName, pkType.UnescapedTableName, pkType.PrimaryKeys.First().EscapedFieldName);
                }

                if (!string.IsNullOrEmpty(dfi.DefaultValue))
                    sb.AppendFormat("DEFAULT {0}", dfi.DefaultValue);
            }

            sb.AppendFormat(")"); //close table(
            cmd.CommandText = sb.ToString();
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
            OracleCommand scmd = new OracleCommand();
            scmd.CommandText = string.Format("ALTER TABLE {0} DROP COLUMN [{1}]", ResolveTableName(type, false), dfi.FieldName);
            return scmd;
        }

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public override IDbCommand GetAddColumnCommnad(TypeInfo type, DataFieldInfo dfi)
        {
            OracleCommand scmd = new OracleCommand();
            scmd.CommandText = string.Format("ALTER TABLE {0} add ({1} {2})", ResolveTableName(type, false), dfi.FieldName, TranslateTypeToSql(dfi));
            return scmd;
        }        

        public override IDbCommand GetModifyColumnCommand(TypeInfo type, DataFieldInfo dfi, string targetFieldType)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.CommandText = string.Format("ALTER TABLE {0} ALTER COLUMN [{1}] {2}", ResolveTableName(type), dfi.FieldName, targetFieldType);
            return cmd;

        }

        public override string ResolveTableName(TypeInfo ti, bool EscapeTableName)
        {
            return ti.UnescapedTableName;
        }

        public override string TranslateTypeToSql(DataFieldInfo dfi)
        {
            string toReturn = dfi.DataFieldType;
            if (string.IsNullOrEmpty(toReturn)) // no user specified, generate column type
            {
                if (dfi.PrimaryKeyType != null)
                { //use the primary keys field type instead					
                    toReturn = TranslateTypeToSql(DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType).PrimaryKeys.First());
                }
                else
                {
                    if (dfi.PropertyType.IsGenericType && dfi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        dfi.PropertyType = dfi.PropertyType.GetGenericArguments()[0];

                    string toChange = dfi.PropertyType.Name.ToUpper();
                    switch (toChange)
                    {
                        case "STRING":
                            toReturn = "varchar(200)";
                            break;
                        case "INT32":
                        case "INT64":
                        case "SINGLE":
                        case "DOUBLE":
                            toReturn = "number";
                            break;
                        case "BYTE[]":
                            toReturn = "BLOB";
                            break;
                        case "BOOLEAN":
                            toReturn = "number";
                            break;
                        case "DATETIME":
                        case "DATETIMEOFFSET":
                        case "TIMESPAN":
                            toReturn = "INTERVAL DAY TO SECOND";
                            break;
                        case "CHAR":
                            toReturn = "varchar(1)";
                            break;

                    }
                }
            }
            return toReturn;       
        }
    }
}
