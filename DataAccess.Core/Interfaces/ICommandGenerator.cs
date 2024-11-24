using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using DataAccess.Core.Data;
using System.Linq.Expressions;
using System.Data.Common;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Generates various types of data store Commands
    /// </summary>
    public interface ICommandGenerator
    {
        TypeParser TypeParser { get; }

        /// <summary>
        /// Returns a command for inserting one object
        /// </summary>
        /// <param name="item">The object to insert</param>
        /// <returns></returns>
        DbCommand GetInsertCommand(object item);

        /// <summary>
        /// Returns a command for inserting a list of objects
        /// </summary>
        /// <param name="items">The objects to insert</param>
        /// <returns></returns>
        DbCommand GetInsertCommand(IList items);

        /// <summary>
        /// Returns a command for performing an update on an object
        /// </summary>
        /// <param name="item">The object to update</param>
        /// <returns></returns>
        DbCommand GetUpdateCommand(object item);

        /// <summary>
        /// Generates a select for a single object
        /// </summary>
        /// <param name="item">The item to load (primary key needs to be set)</param>
        /// <returns></returns>
        DbCommand GetSelectCommand(object item);

        /// <summary>
        /// Generates a delete command for one object (primary key is required)
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        DbCommand GetDeleteCommand(object item);

        /// <summary>
        /// Generates a command appropriate for loading an entire table from the data store
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        DbCommand LoadEntireTableCommand(Type item);

        /// <summary>
        /// Returns a command for creating a new table
        /// </summary>
        /// <param name="ti">The type to create a table for</param>
        /// <returns></returns>
        IEnumerable<DbCommand> GetAddTableCommand(DatabaseTypeInfo ti);

        /// <summary>
        /// Returns a command for removing a column from a table
        /// </summary>
        /// <param name="type">The type to remove the column from</param>
        /// <param name="dfi">The column to remove</param>
        /// <returns></returns>
        DbCommand GetRemoveColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for adding a column to a table
        /// </summary>
        /// <param name="type">The type to add the column to</param>
        /// <param name="dfi">The column to add</param>
        /// <returns></returns>
        IEnumerable<DbCommand> GetAddColumnCommnad(DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <returns></returns>
        IEnumerable<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi);

        /// <summary>
        /// Returns a command for modifying a column to the specified type
        /// </summary>
        /// <param name="type">The type to modify</param>
        /// <param name="dfi">The column to modify</param>
        /// <param name="targetFieldType">The type to change the field to</param>
        /// <returns></returns>
        IEnumerable<DbCommand> GetModifyColumnCommand(DatabaseTypeInfo type, DataFieldInfo dfi, string targetFieldType);

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
        /// <returns></returns>
        string GetSelectList(Type type);

        /// <summary>
        /// Translates a type to SQL equivalent
        /// </summary>
        /// <param name="dfi">The data field.</param>
        /// <returns></returns>
        string TranslateTypeToSql(DataFieldInfo dfi);

        /// <summary>
        /// Returns a command that is appropriate for adding a schema for the object to go into
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        DbCommand GetAddSchemaCommand(DatabaseTypeInfo ti);
    }
}
