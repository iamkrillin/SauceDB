using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System.Collections;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core
{
    /// <summary>
    /// Generates various types of data store Commands
    /// </summary>
    public abstract class DatabaseCommandGenerator : ICommandGenerator
    {
        /// <summary>
        /// The data store this instance is using
        /// </summary>
        /// <value></value>
        public IDataStore DataStore { set; protected get; }

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        public abstract IEnumerable<IDbCommand> GetAddTableCommand(DatabaseTypeInfo ti);

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        public abstract IDbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        public abstract IEnumerable<IDbCommand> GetAddColumnCommnad(DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command appropriate for adding a schema
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        public abstract IDbCommand GetAddSchemaCommand(DatabaseTypeInfo ti);

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <param name="targetFieldType">The type to change the field to</param>
        /// <returns></returns>
        public abstract IEnumerable<IDbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType);

        /// <summary>
        /// Generates a command appropriate for loading an entire table from the data store
        /// </summary>
        /// <param name="item">The type to load</param>
        /// <returns></returns>
        public virtual IDbCommand LoadEntireTableCommand(Type item)
        {
            IDbCommand command = DataStore.Connection.GetCommand();
            command.CommandText = string.Format("SELECT {0} FROM {1};", GetSelectList(item, false), ResolveTableName(item));
            return command;
        }

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <returns></returns>
        public virtual IEnumerable<IDbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi)
        {
            return GetModifyColumnCommand(type, dfi, TranslateTypeToSql(dfi));
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
                if (i > 0) sb.Append(",");
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
                if (i > 0) sb.Append(",");
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
        public virtual IDbCommand GetInsertCommand(object item)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(item.GetType());
            IDbCommand cmd = null;
            if (item != null)
            {
                cmd = DataStore.Connection.GetCommand();
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
        public virtual IDbCommand GetInsertCommand(IList items)
        {
            IDbCommand cmd = null;
            //remove null items
            while (items.Contains(null))
                items.Remove(null);

            if (items.Count > 0)
            {
                DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(items[0].GetType());
                cmd = DataStore.Connection.GetCommand();
                string fieldList = null;
                StringBuilder parmbuilder = new StringBuilder();

                foreach (object o in items)
                {
                    List<ParameterData> parms = GetObjectParameters(cmd.Parameters.Count, o, ti);
                    if (fieldList == null) fieldList = BuildFieldList(parms);
                    if (parmbuilder.Length > 0) parmbuilder.Append(",");
                    parmbuilder.Append("(");
                    parmbuilder.Append(AppendParameters(parms, cmd));
                    parmbuilder.Append(")");
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
        public virtual IDbCommand GetUpdateCommand(object item)
        {
            DatabaseTypeInfo data = DataStore.TypeInformationParser.GetTypeInfo(item.GetType());
            IDbCommand toReturn = DataStore.Connection.GetCommand();
            StringBuilder fieldList = new StringBuilder("UPDATE ");
            fieldList.Append(ResolveTableName(data));
            fieldList.Append(" SET ");
            bool addComma = false;

            foreach (DataFieldInfo dfi in data.DataFields)
            {
                if (!dfi.PrimaryKey && dfi.SetOnInsert)
                {
                    if (addComma) fieldList.Append(",");
                    object value = dfi.Getter(item, null);

                    if (value != null)
                    {
                        string fName = GetParameterName(toReturn);
                        fieldList.Append(string.Concat(dfi.EscapedFieldName, "=", fName));
                        toReturn.Parameters.Add(DataStore.Connection.GetParameter(fName, value));
                    }
                    else
                    {
                        fieldList.Append(string.Format(@"{0}=null", dfi.EscapedFieldName));
                    }
                    addComma = true;
                }
            }

            fieldList.Append(" WHERE ");
            for (int i = 0; i < data.PrimaryKeys.Count(); i++)
            {
                DataFieldInfo dField = data.PrimaryKeys.ElementAt(i);
                if (i > 0) fieldList.Append(" AND ");

                string fName = GetParameterName(toReturn);
                fieldList.Append(string.Concat(dField.EscapedFieldName, "=", fName));
                toReturn.Parameters.Add(DataStore.Connection.GetParameter(fName, dField.Getter(item, null)));
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
            List<ParameterData> toReturn = new List<ParameterData>();
            foreach (DataFieldInfo info in TypeInfo.DataFields)
            {
                if (info.SetOnInsert)
                {
                    object value = info.Getter(item, null);
                    if (value != null)
                    {
                        value = DataStore.Connection.CLRConverter.ConvertToType(value, info.PropertyType);
                        IDbDataParameter parm = DataStore.Connection.GetParameter((++startingIndex).ToString(), DataStore.Connection.DatastoreConverter.CoerceValue(value));
                        toReturn.Add(new ParameterData(parm, info.EscapedFieldName));
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Generates a select for a single object
        /// </summary>
        /// <param name="item">The item to load (primary key needs to be set)</param>
        /// <param name="LoadAllFields">if true, the load field on type info will be ignored</param>
        /// <returns></returns>
        public virtual IDbCommand GetSelectCommand(object item, bool LoadAllFields)
        {
            Type t = item.GetType();
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(t);
            IDbCommand cmd = DataStore.Connection.GetCommand();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT {0} FROM {1} WHERE ", GetSelectList(t, LoadAllFields), ResolveTableName(ti));

            for (int i = 0; i < ti.PrimaryKeys.Count; i++)
            {
                DataFieldInfo dfi = ti.PrimaryKeys[i];
                object value = dfi.Getter(item, null);

                if (value != null)
                {
                    if (i > 0) sb.Append(" AND ");
                    string pName = GetParameterName(cmd);
                    sb.Append(string.Concat(dfi.EscapedFieldName, "=", pName));
                    cmd.Parameters.Add(DataStore.Connection.GetParameter(pName, value));
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
        public virtual IDbCommand GetDeleteCommand(object item)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(item.GetType());
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            sb.Append(ResolveTableName(ti));
            sb.Append(" WHERE ");
            IDbCommand cmd = DataStore.Connection.GetCommand();
            foreach (DataFieldInfo dfi in ti.PrimaryKeys)
            {
                string pName = GetParameterName(cmd);

                if (cmd.Parameters.Count > 0) sb.Append(" AND ");
                sb.Append(string.Concat(dfi.EscapedFieldName, "=", pName));
                cmd.Parameters.Add(DataStore.Connection.GetParameter(pName, dfi.Getter(item, null)));
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
        public virtual IEnumerable<DataFieldInfo> GetSelectFields(Type type, bool LoadAllFields)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(type);

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
        /// <param name="LoadAllFields">Honor LoadFieldAttribute</param>
        /// <returns></returns>
        public virtual string GetSelectList(Type type, bool LoadAllFields)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(type);
            if (ti.SelectString == null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataFieldInfo dfi in ti.DataFields)
                {
                    if (!dfi.LoadField && !LoadAllFields) continue;
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(ResolveFieldName(dfi.PropertyName, type));
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
        public virtual string ResolveTableName(Type type)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(type);
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
                toReturn = string.Format("{0}{1}_{2}{3}", EscapeTableName ? DataStore.Connection.LeftEscapeCharacter : "", ti.UnEscapedSchema, ti.UnescapedTableName, EscapeTableName ? DataStore.Connection.RightEscapeCharacter : "");

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
        public virtual string ResolveFieldName(string PropertyName, Type type)
        {
            string toReturn = "";
            DataFieldInfo dfi = DataStore.TypeInformationParser.GetTypeFields(type).Where(R => R.PropertyName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (dfi != null)
                toReturn = dfi.EscapedFieldName;

            return toReturn;
        }

        /// <summary>
        /// Translates a type to SQL equivalent
        /// </summary>
        /// <param name="dfi">The data field.</param>
        /// <returns></returns>
        public virtual string TranslateTypeToSql(DataFieldInfo dfi)
        {
            if (dfi.PrimaryKeyType != null) //if there is primary key, the field types MUST match
                return TranslateTypeToSql(DataStore.TypeInformationParser.GetTypeInfo(dfi.PrimaryKeyType).PrimaryKeys.First());
            else
            {
                if (dfi.DataFieldType != Attributes.FieldType.Default)
                {
                    return DataStore.Connection.DatastoreConverter.MapFieldType(dfi.DataFieldType, dfi);
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
                        return DataStore.Connection.DatastoreConverter.MapType(targetType, dfi);
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
            switch (foreignKeyType)
            {
                case ForeignKeyType.Cascade:
                    return "Cascade";
                case ForeignKeyType.NoAction:
                    return "No Action";
                case ForeignKeyType.SetNull:
                    return "Set Null";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Generates an IN() clause
        /// </summary>
        /// <param name="fieldName">The fields name</param>
        /// <param name="objects">The objects to select on</param>
        /// <param name="appendTo">The string builder to append to</param>
        /// <param name="cmdAppend">The command to add the parameters to</param>
        /// <param name="Type">The Type of constraint, defaults to AND</param>
        /// <returns></returns>
        public virtual bool AppendInClause(string fieldName, IList objects, StringBuilder appendTo, IDbCommand cmdAppend, ConstraintType Type = ConstraintType.AND)
        {
            bool added = false;
            if (objects.Count > 0)
            {
                if (ConstraintNeeded(appendTo, cmdAppend))
                    appendTo.Append(string.Concat(" ", Type.ToString(), " "));

                EscapeFieldName(fieldName, appendTo);
                appendTo.Append(" IN (");
                for (int i = 0; i < objects.Count; i++)
                {
                    if (i > 0)
                        appendTo.Append(", ");

                    string lvlParm = GetParameterName(cmdAppend);
                    appendTo.Append(lvlParm);
                    IDbDataParameter dbParm = cmdAppend.CreateParameter();
                    dbParm.ParameterName = lvlParm;
                    dbParm.Value = objects[i];
                    cmdAppend.Parameters.Add(dbParm);
                }
                appendTo.Append(")");
                added = true;
            }
            return added;
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
                if (i > 0) appendTo.Append(".");
                appendTo.Append(string.Concat(DataStore.Connection.LeftEscapeCharacter, parts[i], DataStore.Connection.RightEscapeCharacter));
            }
        }

        /// <summary>
        /// Appends a restraint to a where, note it will add a comma if needed
        /// </summary>
        /// <param name="field">The field</param>
        /// <param name="value">The value of the field</param>
        /// <param name="type">The type of constraint</param>
        /// <param name="appendTo">The string builder to append to</param>
        /// <param name="cmdAppend">the command to add the parameters to</param>
        /// <param name="Type">The Type of constraint, defaults to AND</param>
        /// <returns></returns>
        public virtual bool AppendWhereItem(string field, object value, WhereType type, StringBuilder appendTo, IDbCommand cmdAppend, ConstraintType Type = ConstraintType.AND)
        {
            bool append = false;
            string parmName = GetParameterName(cmdAppend);

            if (value != null)
                append = true;
            else if (value is string)
                append = !string.IsNullOrEmpty((string)value);

            if (append)
            {
                if (ConstraintNeeded(appendTo, cmdAppend))
                    appendTo.Append(string.Concat(" ", Type.ToString(), " "));

                appendTo.Append(" ");
                EscapeFieldName(field, appendTo);
                appendTo.Append(GenWhere(parmName, type));
                IDbDataParameter parm = cmdAppend.CreateParameter();
                parm.Value = value;
                parm.ParameterName = parmName;
                cmdAppend.Parameters.Add(parm);
            }
            return append;
        }

        private bool ConstraintNeeded(StringBuilder appendTo, IDbCommand cmd)
        {
            bool result = false;
            if (cmd.Parameters.Count > 0)
            {// should never need the constraint if there is not at least one parm present...
                char examine = GetLastNonEmptyCharacter(appendTo);
                if (examine != '(')
                    result = true;
            }

            return result;
        }

        private char GetLastNonEmptyCharacter(StringBuilder Builder)
        {
            int offset = 0;
            char toReturn = ' ';
            do
            {
                offset++;
                toReturn = Builder[Builder.Length - offset];
            } while (toReturn == ' ');

            return toReturn;
        }

        /// <summary>
        /// Escapes the field and puts the pieces of the where together
        /// </summary>
        /// <param name="parm">The parm.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected virtual string GenWhere(string parm, WhereType type)
        {
            switch (type)
            {
                case WhereType.Contains:
                    return string.Concat(" LIKE '%' + ", parm, " + '%'");
                case WhereType.EndsWith:
                    return string.Concat(" LIKE '%' + ", parm);
                case WhereType.StartsWith:
                    return string.Concat(" LIKE ", parm, " + '%'");
                default:
                    return string.Concat(type.GetWhereString(), parm);
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

        /// <summary>
        /// Generates a delete command from an expression
        /// </summary>
        /// <param name="criteria">The expression that points to the item to remove</param>
        /// <returns></returns>
        public virtual IDbCommand GetDeleteCommand<T>(Expression<Func<T, bool>> criteria)
        {
            DatabaseTypeInfo ti = DataStore.TypeInformationParser.GetTypeInfo(typeof(T));
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            sb.Append(ResolveTableName(ti));
            sb.Append(" WHERE ");
            IDbCommand cmd = DataStore.Connection.GetCommand();

            IDeleteFormatter formatter = DataStore.Connection.GetDeleteFormatter(DataStore);
            Dictionary<string, object> whereParams = new Dictionary<string, object>();
            string whereString = formatter.FormatDelete(DataAccess.Core.Linq.Common.PartialEvaluator.Eval(criteria), out whereParams);

            foreach (KeyValuePair<string, object> par in whereParams)
                cmd.Parameters.Add(DataStore.Connection.GetParameter(par.Key, par.Value));

            sb.Append(whereString);

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        /// <summary>
        /// Returns a command that will do an IN
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public virtual IDbCommand GetInCommand<T>(IEnumerable Ids)
        {
            DatabaseTypeInfo data = DataStore.TypeInformationParser.GetTypeInfo(typeof(T));
            IDbCommand toReturn = DataStore.Connection.GetCommand();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append(GetSelectList(data.DataType, false));
            sb.Append(" FROM ");
            sb.Append(ResolveTableName(data.DataType));
            sb.Append(" WHERE ");
            sb.Append(data.PrimaryKeys.First().EscapedFieldName);
            sb.Append(" IN(");
            foreach (object o in Ids)
            {
                string name = GetParameterName(toReturn);
                toReturn.Parameters.Add(DataStore.Connection.GetParameter(name, o));
                sb.Append(string.Concat(name, ","));
            }

            sb.Remove(sb.Length - 1, 1); //remove trialing comma
            sb.Append(");");

            toReturn.CommandText = sb.ToString();
            return toReturn;
        }
    }
}
