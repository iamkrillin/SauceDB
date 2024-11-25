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
using System.Data.Common;
using System.Threading.Tasks;

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
        Task<IQueryData> ExecuteQuery(DbCommand command);

        /// <summary>
        /// Gets or sets the DataConnection
        /// </summary>
        IDataConnection Connection { get; }

        /// <summary>
        /// Gets or sets the command executor
        /// </summary>
        IExecuteDatabaseCommand ExecuteCommands { get; set; }

        /// <summary>
        /// Gets or sets the schema validator.
        /// </summary>
        /// <value>The schema validator.</value>
        ISchemaValidator SchemaValidator { get; set; }

        /// <summary>
        /// Determines how to search for data store objects based on a CLR type
        /// </summary>
        IFindDataObjects ObjectFinder { get; set; }

        TypeParser TypeParser { get; }

        /// <summary>
        /// Inserts an object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        Task<bool> InsertObject(object item);

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        Task<T> LoadObject<T>(object PrimaryKey);

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <param name="dtoType">The type to load.</param>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        Task<object> LoadObject(Type dtoType, object PrimaryKey);

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        Task<bool> DeleteObject(object item);

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <param name="pkey">The key to delete on</param>
        /// <returns></returns>
        Task<bool> DeleteObject<T>(object pkey);

        /// <summary>
        /// Deletes objects based on an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        Task<int> DeleteObjects<T>(Expression<Func<T, bool>> criteria);

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <param name="item">The type to load</param>
        IAsyncEnumerable<object> LoadEntireTable(Type item);

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        IAsyncEnumerable<T> LoadEntireTable<T>();

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns></returns>
        Task<string> GetTableName(Type t);

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        Task<string> GetTableName<T>();

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <param name="objectType">The type to load in the list</param>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        IAsyncEnumerable<object> ExecuteCommandLoadList(Type objectType, DbCommand command);

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <typeparam name="T">The type to load in the list</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        IAsyncEnumerable<T> ExecuteCommandLoadList<T>(DbCommand command);        

        /// <summary>
        /// Execute a command an loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        Task<T> ExecuteCommandLoadObject<T>(DbCommand command);

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
        Task<object> GetKeyForItemType(Type type, object item);

        /// <summary>
        /// Loads an object from the data store, the key must be set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> LoadObject(object item);

        /// <summary>
        /// Inserts a list of items into the data store
        /// </summary>
        /// <param name="items">The items to insert</param>
        /// <returns></returns>
        Task<bool> InsertObjects(IList items);

        /// <summary>
        /// Determines if an object already exists in the data store, based on the primary key
        /// </summary>
        /// <param name="item">The object to check</param>
        /// <returns></returns>
        Task<bool> IsNew(object item);

        /// <summary>
        /// Will do an insert or update as appropriate to persist your data
        /// Note: This method will determine what operation is required by calling IsNew()
        /// </summary>
        /// <param name="item">The item to persist</param>
        Task<bool> SaveObject(object item);

        /// <summary>
        /// Updates an object on the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns></returns>
        Task<bool> UpdateObject(object item);

        /// <summary>
        /// Updates all items in the list, primary keys must be set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns>The items that failed to update</returns>
        IAsyncEnumerable<T> UpdateObjects<T>(IEnumerable<T> items);

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
       Task<int> ExecuteCommand(DbCommand command);

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
