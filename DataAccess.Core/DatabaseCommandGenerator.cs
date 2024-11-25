using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System.Collections;
using System.Data.Common;
using System.Threading.Tasks;

namespace DataAccess.Core
{
    /// <summary>
    /// Generates various types of data store Commands
    /// </summary>
    public abstract class DatabaseCommandGenerator : ICommandGenerator
    {
        private IDataConnection _connection;

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public abstract Task<List<DbCommand>> GetAddTableCommand(TypeParser tParser, DatabaseTypeInfo ti);

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public abstract DbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public abstract Task<List<DbCommand>> GetAddColumnCommnad(TypeParser tparser, DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command appropriate for adding a schema
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public abstract DbCommand GetAddSchemaCommand(DatabaseTypeInfo ti);

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <param name="targetFieldType">The type to change the field to</param>
        /// <returns></returns>
        public abstract List<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType);

        public DatabaseCommandGenerator(IDataConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Generates a command appropriate for loading an entire table from the data store
        /// </summary>
        /// <param name="item">The type to load</param>
        /// <returns></returns>
        public virtual async Task<DbCommand> LoadEntireTableCommand(TypeParser tParser, Type item)
        {
            string sList = await GetSelectList(tParser, item);
            string tName = await ResolveTableName(tParser, item);

            DbCommand command = _connection.GetCommand();
            command.CommandText = $"SELECT {sList} FROM {tName};";
            return command;
        }

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <returns></returns>
        public virtual async Task<List<DbCommand>> GetModifyColumnCommand(TypeParser tParser, DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            var item = await TranslateTypeToSql(tParser, dfi);
            return GetModifyColumnCommand(type, dfi, item);
        }

        /// <summary>
        /// Builds the field list.
        /// </summary>
        /// <param name="parms">The parms.</param>
        /// <returns></returns>
        protected virtual string BuildFieldList(List<ParameterData> parms)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parms.Count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.AppendFormat(parms[i].Field);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Appends the parameters.
        /// </summary>
        /// <param name="parms">The parms.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected virtual string AppendParameters(List<ParameterData> parms, IDbCommand command)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parms.Count; i++)
            {
                if (i > 0) sb.Append(',');
                if (parms[i].Parameter.Value == null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.Append(string.Concat("@", parms[i].Parameter.ParameterName));
                    command.Parameters.Add(parms[i].Parameter);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        public virtual async Task<DbCommand> GetInsertCommand(TypeParser tParser, object item)
        {
            DatabaseTypeInfo ti = await tParser.GetTypeInfo(item.GetType());
            DbCommand cmd = null;
            if (item != null)
            {
                cmd = _connection.GetCommand();
                List<ParameterData> parms = GetObjectParameters(0, item, ti);
                string fieldList = BuildFieldList(parms);
                string parmList = AppendParameters(parms, cmd);
                cmd.CommandText = string.Concat("INSERT INTO ", ResolveTableName(ti), "(", fieldList, ") VALUES(", parmList, ");");
            }
            return cmd;
        }

        /// <summary>
        /// Returns a command for inserting a list of objects
        /// </summary>
        /// <param name="items">The objects to insert</param>
        /// <returns></returns>
        public virtual async Task<DbCommand> GetInsertCommand(TypeParser tParser, IList items)
        {
            DbCommand cmd = null;

            //remove null items
            while (items.Contains(null))
                items.Remove(null);

            if (items.Count > 0)
            {
                DatabaseTypeInfo ti = await tParser.GetTypeInfo(items[0].GetType());
                cmd = _connection.GetCommand();
                string fieldList = null;
                StringBuilder parmbuilder = new StringBuilder();

                for (int i = 0; i < items.Count; i++)
                {
                    object item = items[i];
                    List<ParameterData> parms = GetObjectParameters((ti.DataFields.Count * i), item, ti);
                    fieldList ??= BuildFieldList(parms);

                    if (parmbuilder.Length > 0)
                        parmbuilder.Append(',');

                    parmbuilder.Append('(');
                    parmbuilder.Append(AppendParameters(parms, cmd));
                    parmbuilder.Append(')');
                }

                cmd.CommandText = string.Concat("INSERT INTO ", ResolveTableName(ti), "(", fieldList, ") VALUES", parmbuilder.ToString(), ";");
            }
            return cmd;
        }

        /// <summary>
        /// Returns a command for performing an update on an object
        /// </summary>
        /// <param name="item">The object to update</param>
        /// <returns></returns>
        public virtual async Task<DbCommand> GetUpdateCommand(TypeParser tParser, object item)
        {
            DatabaseTypeInfo data = await tParser.GetTypeInfo(item.GetType());
            DbCommand toReturn = _connection.GetCommand();
            StringBuilder fieldList = new StringBuilder("UPDATE ");
            fieldList.Append(ResolveTableName(data));
            fieldList.Append(" SET ");
            bool addComma = false;

            foreach (DataFieldInfo dfi in data.DataFields)
            {
                if (!dfi.PrimaryKey && dfi.SetOnInsert)
                {
                    if (addComma) fieldList.Append(',');
                    object value = dfi.Getter(item);

                    if (value != null)
                    {
                        string fName = GetParameterName(toReturn);
                        fieldList.Append(string.Concat(dfi.EscapedFieldName, "=", fName));
                        toReturn.Parameters.Add(_connection.GetParameter(fName, value));
                    }
                    else
                    {
                        fieldList.Append(string.Format(@"{0}=null", dfi.EscapedFieldName));
                    }
                    addComma = true;
                }
            }

            fieldList.Append(" WHERE ");
            for (int i = 0; i < data.PrimaryKeys.Count; i++)
            {
                DataFieldInfo dField = data.PrimaryKeys.ElementAt(i);
                if (i > 0) fieldList.Append(" AND ");

                string fName = GetParameterName(toReturn);
                fieldList.Append(string.Concat(dField.EscapedFieldName, "=", fName));
                toReturn.Parameters.Add(_connection.GetParameter(fName, dField.Getter(item)));
            }

            toReturn.CommandText = fieldList.ToString();
            return toReturn;
        }

        /// <summary>
        /// Gets the object parameters.
        /// </summary>
        /// <param name="startingIndex">The number of parameters you already have.</param>
        /// <param name="item">The item.</param>
        /// <param name="TypeInfo">The type info.</param>
        /// <returns></returns>
        protected virtual List<ParameterData> GetObjectParameters(int startingIndex, object item, DatabaseTypeInfo TypeInfo)
        {
            List<ParameterData> toReturn = [];
            foreach (DataFieldInfo info in TypeInfo.DataFields)
            {
                if (info.SetOnInsert)
                {
                    object value = info.Getter(item);
                    value = _connection.CLRConverter.ConvertToType(value, info.PropertyType);
                    IDbDataParameter parm = _connection.GetParameter((++startingIndex).ToString(), _connection.DatastoreConverter.CoerceValue(value));
                    toReturn.Add(new ParameterData(parm, info.EscapedFieldName));
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Generates a select for a single object
        /// </summary>
        /// <param name="item">The item to load (primary key needs to be set)</param>
        /// <returns></returns>
        public virtual async Task<DbCommand> GetSelectCommand(TypeParser tParser, object item)
        {
            Type t = item.GetType();
            DatabaseTypeInfo ti = await tParser.GetTypeInfo(t);
            DbCommand cmd = _connection.GetCommand();

            StringBuilder sb = new StringBuilder();
            string sList = await GetSelectList(tParser, t);


            sb.AppendFormat("SELECT {0} FROM {1} WHERE ", sList, ResolveTableName(ti));

            for (int i = 0; i < ti.PrimaryKeys.Count; i++)
            {
                DataFieldInfo dfi = ti.PrimaryKeys[i];
                object value = dfi.Getter(item);

                if (value != null)
                {
                    if (i > 0) sb.Append(" AND ");
                    string pName = GetParameterName(cmd);
                    sb.Append(string.Concat(dfi.EscapedFieldName, "=", pName));
                    cmd.Parameters.Add(_connection.GetParameter(pName, value));
                }
            }

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        /// <summary>
        /// Generates a delete command for one object (primary key is required)
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        public virtual async Task<DbCommand> GetDeleteCommand(TypeParser tParser, object item)
        {
            DatabaseTypeInfo ti = await tParser.GetTypeInfo(item.GetType());
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            sb.Append(ResolveTableName(ti));
            sb.Append(" WHERE ");
            DbCommand cmd = _connection.GetCommand();
            foreach (DataFieldInfo dfi in ti.PrimaryKeys)
            {
                string pName = GetParameterName(cmd);

                if (cmd.Parameters.Count > 0) sb.Append(" AND ");
                sb.Append(string.Concat(dfi.EscapedFieldName, "=", pName));
                cmd.Parameters.Add(_connection.GetParameter(pName, dfi.Getter(item)));
            }

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        /// <summary>
        /// Returns a list of columns, appropriate for selecting
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="LoadAllFields">Honor LoadFieldAttribute</param>
        /// <returns></returns>
        public virtual async IAsyncEnumerable<DataFieldInfo> GetSelectFields(TypeParser tparser, Type type, bool LoadAllFields)
        {
            DatabaseTypeInfo ti = await tparser.GetTypeInfo(type);

            foreach (DataFieldInfo dfi in ti.DataFields)
            {
                if (!dfi.LoadField && !LoadAllFields) continue;
                yield return dfi;
            }
        }

        /// <summary>
        /// Returns a list of columns comma separated, appropriate for select from
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public virtual async Task<string> GetSelectList(TypeParser tParser, Type type)
        {
            DatabaseTypeInfo ti = await tParser.GetTypeInfo(type);
            if (ti.SelectString == null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataFieldInfo dfi in ti.DataFields)
                {
                    if (!dfi.LoadField) continue;
                    if (sb.Length > 0) sb.Append(',');
                    sb.Append(await ResolveFieldName(tParser, dfi.PropertyName, type));
                }

                ti.SelectString = sb.ToString();
            }

            return ti.SelectString;
        }

        /// <summary>
        /// Returns the name of the table (schema.table)
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public virtual async Task<string> ResolveTableName(TypeParser tParser, Type type)
        {
            DatabaseTypeInfo ti = await tParser.GetTypeInfo(type);
            return ResolveTableName(ti);
        }

        /// <summary>
        /// Resolves the name of the table.
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <returns></returns>
        public virtual string ResolveTableName(DatabaseTypeInfo ti)
        {
            return ResolveTableName(ti, true);
        }

        /// <summary>
        /// Resolves the name of the table.
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <param name="EscapeTableName">Should the table name be escaped</param>
        /// <returns></returns>
        public virtual string ResolveTableName(DatabaseTypeInfo ti, bool EscapeTableName)
        {
            string toReturn = EscapeTableName ? ti.TableName : ti.UnescapedTableName;

            if (!string.IsNullOrEmpty(ti.Schema))
                toReturn = string.Format("{0}{1}_{2}{3}", EscapeTableName ? _connection.LeftEscapeCharacter : "", ti.UnEscapedSchema, ti.UnescapedTableName, EscapeTableName ? _connection.RightEscapeCharacter : "");

            return toReturn;
        }

        /// <summary>
        /// Resolves the name of the table
        /// </summary>
        /// <param name="schema">The name of the schemaa</param>
        /// <param name="table">The table name</param>
        /// <returns></returns>
        public virtual string ResolveTableName(string schema, string table)
        {
            string toReturn = table;

            if (!string.IsNullOrEmpty(schema))
                toReturn = string.Format("{0}_{1}", schema, table);

            return toReturn;
        }

        /// <summary>
        /// Returns the name of a column
        /// </summary>
        /// <param name="PropertyName">The objects property to use</param>
        /// <param name="type">The type</param>
        /// <returns></returns>
        public virtual async Task<string> ResolveFieldName(TypeParser tParser, string PropertyName, Type type)
        {
            string toReturn = "";
            var fields = await tParser.GetTypeFields(type);

            DataFieldInfo dfi = fields.Where(R => R.PropertyName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (dfi != null)
                toReturn = dfi.EscapedFieldName;

            return toReturn;
        }

        /// <summary>
        /// Translates a type to SQL equivalent
        /// </summary>
        /// <param name="dfi">The data field.</param>
        /// <returns></returns>
        public virtual async Task<string> TranslateTypeToSql(TypeParser tParser, DataFieldInfo dfi)
        {
            if (dfi.PrimaryKeyType != null) //if there is primary key, the field types MUST match
            {
                var typeInfo = await tParser.GetTypeInfo(dfi.PrimaryKeyType);
                return await TranslateTypeToSql(tParser, typeInfo.PrimaryKeys.First());
            }
            else
            {
                if (dfi.DataFieldType != Attributes.FieldType.Default)
                {
                    return _connection.DatastoreConverter.MapFieldType(dfi.DataFieldType, dfi);
                }
                else
                {
                    if (dfi.DataFieldType == Attributes.FieldType.UserString)
                        return dfi.DataFieldString;
                    else
                    {
                        Type targetType = dfi.PropertyType;
                        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            targetType = targetType.GetGenericArguments()[0];

                        if (targetType.IsEnum)
                            targetType = Enum.GetUnderlyingType(targetType);

                        //there is one special case it makes sense to handle here, max length strings...
                        return _connection.DatastoreConverter.MapType(targetType, dfi);
                    }
                }
            }
        }



        /// <summary>
        /// Translates the a foreign key constraint to a sql friendly string
        /// </summary>
        /// <param name="foreignKeyType">Type of the foreign key.</param>
        /// <returns></returns>
        public virtual string TranslateFkeyType(ForeignKeyType foreignKeyType)
        {
            return foreignKeyType switch
            {
                ForeignKeyType.Cascade => "Cascade",
                ForeignKeyType.NoAction => "No Action",
                ForeignKeyType.SetNull => "Set Null",
                _ => "",
            };
        }

        /// <summary>
        /// escapes the field name of a field, will account for an alias/table name or similar
        /// </summary>
        /// <param name="parmName"></param>
        ///<param name="appendTo">The sb to append the field to</param>
        private void EscapeFieldName(string parmName, StringBuilder appendTo)
        {
            string[] parts = parmName.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0) appendTo.Append('.');
                appendTo.Append(string.Concat(_connection.LeftEscapeCharacter, parts[i], _connection.RightEscapeCharacter));
            }
        }

        /// <summary>
        /// Returns a name for the parameter when adding to a command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual string GetParameterName(IDbCommand cmd)
        {
            return string.Concat("@", cmd.Parameters.Count + 1);
        }
    }
}
