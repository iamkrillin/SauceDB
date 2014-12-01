using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using DataAccess.Core.Data;
using System.Linq.Expressions;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Generates various types of data store Commands
    /// </summary>
    public interface ICommandGenerator
    {
        /// <summary>
        /// The data store this instance is using
        /// </summary>
        IDataStore DataStore { set; }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        IDbCommand GetInsertCommand(object item);

        /// <summary>
        /// Returns a command for inserting a list of objects
        /// </summary>
        /// <param name="items">The objects to insert</param>
        /// <returns></returns>
        IDbCommand GetInsertCommand(IList items);

        /// <summary>
        /// Returns a command for performing an update on an object
        /// </summary>
        /// <param name="item">The object to update</param>
        /// <returns></returns>
        IDbCommand GetUpdateCommand(object item);

        /// <summary>
        /// Generates a select for a single object
        /// </summary>
        /// <param name="item">The item to load (primary key needs to be set)</param>
        /// <param name="LoadAllFields">if true, the load field on type info will be ignored</param>
        /// <returns></returns>
        IDbCommand GetSelectCommand(object item, bool LoadAllFields);

        /// <summary>
        /// Generates a delete command for one object (primary key is required)
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        IDbCommand GetDeleteCommand(object item);

        /// <summary>
        /// Generates a command appropriate for loading an entire table from the data store
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IDbCommand LoadEntireTableCommand(Type item);

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        IEnumerable<IDbCommand> GetAddTableCommand(TypeInfo ti);

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        IDbCommand GetRemoveColumnCommand(TypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        IEnumerable<IDbCommand> GetAddColumnCommnad(TypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <returns></returns>
        IEnumerable<IDbCommand> GetModifyColumnCommand(TypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <param name="targetFieldType">The type to change the field to</param>
        /// <returns></returns>
        IEnumerable<IDbCommand> GetModifyColumnCommand(TypeInfo type, DataFieldInfo dfi, string targetFieldType);

        /// <summary>
        /// Returns a delete command appropriate for an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IDbCommand GetDeleteCommand<T>(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Returns the name of the table (schema.table)
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        string ResolveTableName(Type type);

        /// <summary>
        /// Returns the name of the table (schema.table)
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns></returns>
        string ResolveTableName(string schema, string table);

        /// <summary>
        /// Returns the name of a column
        /// </summary>
        /// <param name="PropertyName">The objects property to use</param>
        /// <param name="type">The type</param>
        /// <returns></returns>
        string ResolveFieldName(string PropertyName, Type type);

        /// <summary>
        /// Returns a list of columns comma separated, appropriate for select from
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="LoadAllFields">Honor LoadFieldAttribute</param>
        /// <returns></returns>
        string GetSelectList(Type type, bool LoadAllFields);

        /// <summary>
        /// Returns a list of columns, appropriate for selecting
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="LoadAllFields">Honor LoadFieldAttribute</param>
        /// <returns></returns>
        IEnumerable<DataFieldInfo> GetSelectFields(Type type, bool LoadAllFields);

        /// <summary>
        /// Generates an IN() clause
        /// </summary>
        /// <param name="fieldName">The parameters name</param>
        /// <param name="objects">The objects to select on</param>
        /// <param name="appendTo">The string builder to append to</param>
        /// <param name="cmdAppend">The command to add the parameters to</param>
        /// <param name="Type">The Type of constraint, defaults to AND</param>
        /// <returns></returns>
        bool AppendInClause(string fieldName, IList objects, StringBuilder appendTo, IDbCommand cmdAppend, ConstraintType Type = ConstraintType.AND);

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
        bool AppendWhereItem(string field, object value, WhereType type, StringBuilder appendTo, IDbCommand cmdAppend, ConstraintType Type = ConstraintType.AND);

        /// <summary>
        /// Translates a type to SQL equivalent
        /// </summary>
        /// <param name="dfi">The data field.</param>
        /// <returns></returns>
        string TranslateTypeToSql(DataFieldInfo dfi);

        /// <summary>
        /// Returns a command that will do an IN
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Ids"></param>
        /// <returns></returns>
        IDbCommand GetInCommand<T>(IEnumerable Ids);

        /// <summary>
        /// Returns a command that is appropriate for adding a schema for the object to go into
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        IDbCommand GetAddSchemaCommand(TypeInfo ti);
    }
}
