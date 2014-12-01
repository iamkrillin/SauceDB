using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Linq.Expressions;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using DataAccess.Core.Data;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Mapping;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a data store
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// This event will fire anytime an object is being loaded
        /// </summary>
        event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;

        /// <summary>
        /// This event will fire just before an object is deleted
        /// </summary>
        event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;

        /// <summary>
        /// This event will fire just after an object is deleted
        /// </summary>
        event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;

        /// <summary>
        /// This event will fire just before an object is updated
        /// </summary>
        event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;

        /// <summary>
        /// This event will fire just after an object is updated
        /// </summary>
        event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;

        /// <summary>
        /// This event will fire just before an object is inserted
        /// </summary>
        event EventHandler<ObjectInsertingEventArgs> ObjectInserting;

        /// <summary>
        /// This event will fire just after an object is inserted
        /// </summary>
        event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        IQueryData ExecuteQuery(IDbCommand command);

        /// <summary>
        /// Gets or sets the DataConnection
        /// </summary>
        IDataConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the command executor
        /// </summary>
        IExecuteDatabaseCommand ExecuteCommands { get; set; }

        /// <summary>
        /// Gets or sets the type information parser.
        /// </summary>
        ITypeInformationParser TypeInformationParser { get; set; }

        /// <summary>
        /// Gets or sets the schema validator.
        /// </summary>
        /// <value>The schema validator.</value>
        ISchemaValidator SchemaValidator { get; set; }

        /// <summary>
        /// Determines how to search for data store objects based on a CLR type
        /// </summary>
        IFindDataObjects ObjectFinder { get; set; }

        /// <summary>
        /// Inserts an object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        bool InsertObject(object item);

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <param name="item">The type to load</param>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        object LoadObject(Type item, object PrimaryKey);

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        T LoadObject<T>(object PrimaryKey);

        /// <summary>
        /// Loads a list of objects from the data store
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="Ids">The primary key(s)</param>
        /// <returns></returns>
        IEnumerable<T> LoadObjects<T>(IEnumerable Ids);

        /// <summary>
        /// Loads an object.
        /// </summary>
        /// <param name="item">The type to load.</param>
        /// <param name="key">The primary field</param>
        /// <param name="LoadAllFields">if set to <c>true</c> [The load field attribute tag is ignored].</param>
        object LoadObject(Type item, object key, bool LoadAllFields);

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="PrimaryKey">The primary key</param>
        /// <param name="LoadAllFields">if set to <c>true</c> [The load field attribute tag is ignored].</param>
        T LoadObject<T>(object PrimaryKey, bool LoadAllFields);

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <param name="item">The type to delete</param>
        /// <param name="key">The primary key to delete on</param>
        /// <returns></returns>
        bool DeleteObject(Type item, object key);

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <typeparam name="T">The type to delete</typeparam>
        /// <param name="primaryKey">The primary key to delete on></param>
        /// <returns></returns>
        bool DeleteObject<T>(object primaryKey);

        /// <summary>
        /// Deletes objects based on an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        int DeleteObjects<T>(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <param name="item">The type to load</param>
        IEnumerable<object> LoadEntireTable(Type item);

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        IEnumerable<T> LoadEntireTable<T>();

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns></returns>
        string GetTableName(Type t);

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        string GetTableName<T>();

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <param name="objectType">The type to load in the list</param>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        IEnumerable<object> ExecuteCommandLoadList(Type objectType, IDbCommand command);

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <typeparam name="T">The type to load in the list</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        IEnumerable<T> ExecuteCommandLoadList<T>(IDbCommand command);        

        /// <summary>
        /// Execute a command an loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        T ExecuteCommandLoadObject<T>(IDbCommand command);

        /// <summary>
        /// Returns a helper for dealing with the db directly
        /// </summary>
        /// <typeparam name="T">The type you are dealing with on the return side</typeparam>
        /// <returns></returns>
        DatabaseCommand<T> GetCommand<T>();

        /// <summary>
        /// Returns the key value for an object
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="item">The object to extract from</param>
        /// <returns></returns>
        object GetKeyForItemType(Type type, object item);

        /// <summary>
        /// Loads an object from the data store, the key must be set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool LoadObject(object item);

        /// <summary>
        /// Loads an object from the data store, the key must be set
        /// </summary>
        /// <param name="item">The object to load</param>
        /// <param name="LoadAllFields">If true the loadfield=false will be ignored</param>
        /// <returns></returns>
        bool LoadObject(object item, bool LoadAllFields);

        /// <summary>
        /// Inserts a list of items into the data store
        /// </summary>
        /// <param name="items">The items to insert</param>
        /// <returns></returns>
        bool InsertObjects(IList items);

        /// <summary>
        /// Determines if an object already exists in the data store, based on the primary key
        /// </summary>
        /// <param name="item">The object to check</param>
        /// <returns></returns>
        bool IsNew(object item);

        /// <summary>
        /// Will do an insert or update as appropriate to persist your data
        /// Note: This method will determine what operation is required by calling IsNew()
        /// </summary>
        /// <param name="item">The item to persist</param>
        bool SaveObject(object item);

        /// <summary>
        /// Updates an object on the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns></returns>
        bool UpdateObject(object item);

        /// <summary>
        /// Updates all items in the list, primary keys must be set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns>The items that failed to update</returns>
        IEnumerable<T> UpdateObjects<T>(IEnumerable<T> items);

        /// <summary>
        /// Deletes an objet from the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        bool DeleteObject(object item);

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        int ExecuteCommand(IDbCommand command);

        /// <summary>
        /// Returns a comma separated list of the fields on an object
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns></returns>
        string GetSelectList(Type t);

        /// <summary>
        /// Returns a comma separated list of the fields on an object
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        string GetSelectList<T>();

        /// <summary>
        /// This function will return an IQueryable appropriate for using with LINQ
        /// </summary>
        /// <typeparam name="T">The type to query</typeparam>
        /// <returns></returns>
        IQueryable<T> Query<T>();

        /// <summary>
        /// Returns the query mapper to use when doing LINQ
        /// </summary>
        /// <returns></returns>
        SauceMapping GetQueryMapper();

        /// <summary>
        /// Inits a transction scope for the command executor
        /// </summary>
        TransactionContext StartTransaction();

        /// <summary>
        /// Returns a new instance of a data store on the same connection
        /// </summary>
        /// <returns></returns>
        IDataStore GetNewInstance();
    }
}
