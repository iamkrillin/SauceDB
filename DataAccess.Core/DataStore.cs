using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Execute;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core.ObjectFinders;
using DataAccess.Core.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccess.Core
{
    /// <summary>
    /// A data store
    /// </summary>
    public partial class DataStore : IDataStore
    {
        /// <summary>
        /// Gets or sets the DataConnection
        /// </summary>
        /// <value></value>
        public IDataConnection Connection { get; private set; }

        /// <summary>
        /// Determines how to search for data store objects based on a CLR type
        /// </summary>
        public virtual IFindDataObjects ObjectFinder { get; set; }

        /// <summary>
        /// Gets or sets the command executor
        /// </summary>
        /// <value></value>
        public IExecuteDatabaseCommand ExecuteCommands { get; set; }

        /// <summary>
        /// Gets or sets the schema validator.
        /// </summary>
        /// <value>The schema validator.</value>
        public ISchemaValidator SchemaValidator { get; set; }

        public TypeParser TypeParser { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStore"/> class.
        /// </summary>
        /// <param name="Connection">The data connection</param>
        public DataStore(IDataConnection Connection)
        {
            this.Connection = Connection;
            this.ExecuteCommands = new ExecuteCommands();
            this.SchemaValidator = new ModifySchemaValidator(this);
            this.ObjectFinder = new SupportsSchemaObjectFinder();
            this.TypeParser = new TypeParser(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStore"/> class.
        /// </summary>
        /// <param name="Connection">The data connection.</param>
        /// <param name="ExecuteComamands">The command executor.</param>
        /// <param name="TypeParser">The type parser.</param>
        public DataStore(IDataConnection Connection, IExecuteDatabaseCommand ExecuteComamands)
            : this(Connection)
        {
            this.ExecuteCommands = ExecuteComamands;
        }

        /// <summary>
        /// Loads an object from the data store, the key must be set
        /// </summary>
        /// <param name="item">The object to load</param>
        /// <param name="LoadAllFields">If true the loadfield=false will be ignored</param>
        /// <returns></returns>
        public virtual async Task<bool> LoadObject(object item)
        {
            if (item == null)
                return false;

            DbCommand command = await Connection.CommandGenerator.GetSelectCommand(TypeParser, item);
            return await ProcessCommand(item, command, true);
        }

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        public virtual async Task<T> LoadObject<T>(object PrimaryKey)
        {
            return (T)await LoadObject(typeof(T), PrimaryKey);
        }

        /// <summary>
        /// Loads an object.
        /// </summary>
        /// <param name="item">The type to load.</param>
        /// <param name="key">The primary field</param>
        /// <returns></returns>
        public virtual async Task<object> LoadObject(Type item, object key)
        {
            DatabaseTypeInfo ti = await TypeParser.GetTypeInfo(item);
            if (ti.PrimaryKeys.Count == 1)
            {
                object toReturn = CreateObjectSetKey(item, key, ti);

                if (!await LoadObject(toReturn))
                    toReturn = null;

                return toReturn;
            }
            else
            {
                throw new DataStoreException("This method is only valid when the item contains (1) key");
            }
        }

        /// <summary>
        /// Inserts an object
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertObject(object item)
        {
            if (CheckObjectInserting(item))
            {
                DbCommand command = await Connection.CommandGenerator.GetInsertCommand(TypeParser, item);
                bool result = await ProcessCommand(item, command);
                FireObjectInserted(item, result);
                return result;
            }
            return false;
        }

        /// <summary>
        /// Inserts a list of items into the data store
        /// </summary>
        /// <param name="items">The items to insert</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertObjects(IList items)
        {
            if (items.Count > 0)
            {
                if (items.Count > 50)
                {
                    await Connection.DoBulkInsert(items, this);
                    return true;
                }
                else
                {
                    DbCommand command = await Connection.CommandGenerator.GetInsertCommand(TypeParser, items);
                    return await ProcessCommand(items, command);
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes an objet from the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteObject(object item)
        {
            if (CheckObjectDeleting(item))
            {
                DbCommand command = await Connection.CommandGenerator.GetDeleteCommand(TypeParser, item);
                bool result = await ProcessCommand(item, command);
                FireObjectDeleted(item, result);

                return result;
            }
            else
                return false;
        }

        /// <summary>
        /// Deletes an objet from the data store, primary key must be set
        /// </summary>
        /// <param name="pkey">The key to delete on</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteObject<T>(object pkey)
        {
            object ToRemove = await CreateObjectSetKey(typeof(T), pkey);
            return await DeleteObject(ToRemove);
        }

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <param name="item">The type to load</param>
        /// <returns></returns>
        public virtual async IAsyncEnumerable<object> LoadEntireTable(Type item)
        {
            var command = await Connection.CommandGenerator.LoadEntireTableCommand(TypeParser, item);

            await foreach (var rItem in ExecuteCommandLoadList(item, command))
                yield return rItem;
        }

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <returns></returns>
        public virtual async IAsyncEnumerable<T> LoadEntireTable<T>()
        {
            var command = await Connection.CommandGenerator.LoadEntireTableCommand(TypeParser, typeof(T));

            await foreach (var v in ExecuteCommandLoadList<T>(command))
                yield return v;
        }

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns></returns>
        public virtual async Task<string> GetTableName(Type t)
        {
            return await Connection.CommandGenerator.ResolveTableName(TypeParser, t);
        }

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        public virtual async Task<string> GetTableName<T>()
        {
            return await GetTableName(typeof(T));
        }

        /// <summary>
        /// Returns the key value for an object
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="item">The object to extract from</param>
        /// <returns></returns>
        public virtual async Task<object> GetKeyForItemType(Type type, object item)
        {
            IEnumerable<DataFieldInfo> fields = await TypeParser.GetPrimaryKeys(type);

            if (fields.Count() > 1)
                throw new DataStoreException("This Type contains more than one key");

            if (fields.Count() < 1)
                throw new DataStoreException("This type does not contain a key");

            return fields.ElementAt(0).Getter(item);
        }

        /// <summary>
        /// Determines if an object already exists in the data store, based on the primary key
        /// </summary>
        /// <param name="item">The object to check</param>
        /// <returns></returns>
        public virtual async Task<bool> IsNew(object item)
        {
            DbCommand cmd = await Connection.CommandGenerator.GetSelectCommand(TypeParser, item);
            using (IQueryData qd = await ExecuteCommands.ExecuteCommandQuery(cmd, Connection))
            {
                var enumerator = qd.GetEnumerator();
                enumerator.MoveNext();

                return enumerator.Current == null;
            }
        }

        /// <summary>
        /// Will do an insert or update as appropriate to persist your data
        /// Note: This method will determine what operation is required by calling IsNew()
        /// </summary>
        /// <param name="item"></param>
        public async Task<bool> SaveObject(object item)
        {
            if (await IsNew(item))
                return await InsertObject(item);
            else
                return await UpdateObject(item);
        }

        /// <summary>
        /// Updates an object on the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateObject(object item)
        {
            if (CheckObjectUpdating(item))
            {
                DbCommand command = await Connection.CommandGenerator.GetUpdateCommand(TypeParser, item);
                bool result = await ProcessCommand(command, command);
                FireObjectUpdated(item, result);
                return result;
            }
            return false;
        }

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        public virtual async Task<int> ExecuteCommand(DbCommand command)
        {
            return await ExecuteCommands.ExecuteCommand(command, Connection);
        }

        /// <summary>
        /// Execute a command and loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual async Task<T> ExecuteCommandLoadObject<T>(DbCommand command)
        {
            var item = ExecuteCommandLoadList<T>(command);
            var enumerator = item.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();

            return enumerator.Current;
        }

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <param name="objectType">The type to load in the list</param>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<object> ExecuteCommandLoadList(Type objectType, DbCommand command)
        {
            return ExecuteCommandLoadList<object>(objectType, command);
        }

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <typeparam name="T">The type to load in the list</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<T> ExecuteCommandLoadList<T>(DbCommand command)
        {
            return ExecuteCommandLoadList<T>(typeof(T), command);
        }

        /// <summary>
        /// This function will return an IQueryable appropriate for using with LINQ
        /// </summary>
        /// <typeparam name="T">The type to query</typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> Query<T>()
        {
            IQueryable<T> toReturn = new Query<T>(Connection.GetQueryProvider(this));
            var tInfo = TypeParser.GetTypeInfo(typeof(T));
            tInfo.Wait();

            DatabaseTypeInfo ti = tInfo.Result;

            if (ti.QueryPredicate != null)
                toReturn = ti.QueryPredicate.Invoke(toReturn);

            return toReturn;
        }

        /// <summary>
        /// Returns the query mapper to use when doing LINQ
        /// </summary>
        /// <returns></returns>
        public virtual SauceMapping GetQueryMapper()
        {
            return new SauceMapping(this);
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual async Task<IQueryData> ExecuteQuery(DbCommand command)
        {
            return await ExecuteCommands.ExecuteCommandQuery(command, Connection);
        }

        private async Task<object> BuildObject(IQueryRow dt, DatabaseTypeInfo ti)
        {
            object toReturn = await ObjectBuilder.BuildObject(this, dt, ti);
            FireObjectLoaded(toReturn);
            return toReturn;
        }

        private async Task SetFieldData(DatabaseTypeInfo DatabaseTypeInfo, IQueryRow dt, object p)
        {
            if (dt != null)
            {
                await ObjectBuilder.SetFieldData(this, DatabaseTypeInfo, dt, p);
                FireObjectLoaded(p);
            }
        }

        /// <summary>
        /// Executes a db command and fills in a list of objects with the result data
        /// </summary>
        /// <typeparam name="ReturnType">The type of object to return</typeparam>
        /// <param name="objectType">The type of object to return.</param>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        protected virtual async IAsyncEnumerable<ReturnType> ExecuteCommandLoadList<ReturnType>(Type objectType, DbCommand command)
        {
            DatabaseTypeInfo ti = await TypeParser.GetTypeInfo(objectType);
            using (IQueryData dt = await ExecuteCommands.ExecuteCommandQuery(command, Connection))
            {
                if (dt.QuerySuccessful)
                {
                    foreach (IQueryRow row in dt)
                    {
                        ReturnType toAdd;

                        if (objectType.IsSystemType())
                            toAdd = (ReturnType)Connection.CLRConverter.ConvertToType(row.GetDataForRowField(0), typeof(ReturnType));
                        else
                            toAdd = (ReturnType)await BuildObject(row, ti);

                        yield return toAdd;
                    }
                }
            }
        }

        /// <summary>
        /// Executes a db command and fills in the object, if needed i.e. primary keys on insert etc
        /// </summary>
        /// <param name="item">The item being queried with</param>
        /// <param name="command">The db command</param>
        /// <returns></returns>
        protected virtual async Task<bool> ProcessCommand(object item, DbCommand command)
        {
            return await ProcessCommand(item, command, false);
        }

        /// <summary>
        /// Executes a db command and fills in a list of objects when done, if needed i.e. primary keys on insert etc
        /// </summary>
        /// <param name="item">The item being queried with</param>
        /// <param name="command">The db command</param>
        /// <returns></returns>
        protected virtual async Task<bool> ProcessCommand(IList item, DbCommand command)
        {
            return await ProcessCommand(item, command, false);
        }

        /// <summary>
        /// Executes a db command and fills in a list of objects when done, if needed i.e. primary keys on insert etc
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="command">The db command</param>
        /// <param name="FailIfNoRecords">if set to <c>true</c> and there is no records in the result set, no further processing will be done</param>
        /// <returns></returns>
        protected virtual async Task<bool> ProcessCommand(IList items, DbCommand command, bool FailIfNoRecords)
        {
            return await ProcessCommand(dt =>
            {
                if (items.Count > 0)
                {
                    Type t = items[0].GetType();
                    IEnumerator<IQueryRow> rows = dt.GetQueryEnumerator();

                    for (int i = 0; i < items.Count; i++)
                    {
                        rows.MoveNext();
                        var tInfo = TypeParser.GetTypeInfo(t);
                        tInfo.Wait();

                        SetFieldData(tInfo.Result, rows.Current, items[i]).Wait();
                    }
                    return true;
                }
                return false;
            }, command, FailIfNoRecords);
        }

        /// <summary>
        /// Executes a db command and fills in the object, if needed i.e. primary keys on insert etc
        /// </summary>
        /// <param name="item">The item being queried with</param>
        /// <param name="command">The db command</param>
        /// <param name="FailIfNoRecords">if set to <c>true</c> and there is no records in the result set, no further processing will be done</param>
        /// <returns></returns>
        protected virtual async Task<bool> ProcessCommand(object item, DbCommand command, bool FailIfNoRecords)
        {
            return await ProcessCommand(r =>
            {
                var en = r.GetQueryEnumerator();
                if (en.MoveNext())
                {
                    var tInfo = TypeParser.GetTypeInfo(item.GetType());
                    tInfo.Wait();

                    SetFieldData(tInfo.Result, en.Current, item).Wait();
                    return true;
                }
                else
                    return false;

            }, command, FailIfNoRecords);
        }

        protected virtual async Task<bool> ProcessCommand(Func<IQueryData, bool> OnDone, DbCommand command, bool FailIfNoRecords)
        {
            using (IQueryData dt = await ExecuteCommands.ExecuteCommandQuery(command, Connection))
            {
                if (dt.QuerySuccessful)
                {
                    bool records = OnDone(dt);

                    if (FailIfNoRecords)
                        return records;
                }
                return true;
            }
        }

        /// <summary>
        /// Deletes objects based on an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        public async Task<int> DeleteObjects<T>(Expression<Func<T, bool>> criteria)
        {
            DbCommand cmd = Connection.GetCommand();
            IDeleteFormatter formatter = Connection.GetDeleteFormatter(this);
            string whereString = formatter.FormatDelete(DataAccess.Core.Linq.Common.PartialEvaluator.Eval(criteria), out Dictionary<string, object> whereParams);

            foreach (KeyValuePair<string, object> par in whereParams)
                cmd.Parameters.Add(Connection.GetParameter(par.Key, par.Value));

            string tableName = await Connection.CommandGenerator.ResolveTableName(TypeParser, typeof(T));
            cmd.CommandText = $"DELETE FROM {tableName} WHERE {whereString}";
            return await ExecuteCommands.ExecuteCommand(cmd, Connection);
        }

        /// <summary>
        /// Inits a transaction scope for the command executor
        /// </summary>
        /// <returns></returns>
        public virtual TransactionContext StartTransaction()
        {
            TransactionContext toReturn = new TransactionContext(this);

            //gotta marshal the events so they will propagate right from the transaction context
            toReturn.Instance.ObjectDeleted += (sender, args) => { ObjectDeleted?.Invoke(sender, args); }; ;
            toReturn.Instance.ObjectDeleting += (sender, args) => { ObjectDeleting?.Invoke(sender, args); };
            toReturn.Instance.ObjectInserted += (sender, args) => { ObjectInserted?.Invoke(sender, args); };
            toReturn.Instance.ObjectInserting += (sender, args) => { ObjectInserting?.Invoke(sender, args); };
            toReturn.Instance.ObjectLoaded += (sender, args) => { ObjectLoaded?.Invoke(sender, args); };
            toReturn.Instance.ObjectUpdated += (sender, args) => { ObjectUpdated?.Invoke(sender, args); };
            toReturn.Instance.ObjectUpdating += (sender, args) => { ObjectUpdating?.Invoke(sender, args); };

            return toReturn;
        }

        /// <summary>
        /// Returns a new instance of a data store on the same connection
        /// </summary>
        /// <returns></returns>
        public virtual IDataStore GetNewInstance()
        {
            return new DataStore(Connection, ExecuteCommands)
            {
                SchemaValidator = SchemaValidator,
                ExecuteCommands = ExecuteCommands,
                ObjectFinder = ObjectFinder
            };
        }

        /// <summary>
        /// Returns a helper for dealing with the db directly
        /// </summary>
        /// <typeparam name="T">The type you are dealing with on the return side</typeparam>
        /// <returns></returns>
        public virtual DatabaseCommand<T> GetCommand<T>()
        {
            return new DatabaseCommand<T>(this);
        }

        /// <summary>
        /// Updates all items in the list, primary keys must be set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns>
        /// The items that failed to update
        /// </returns>
        public virtual async IAsyncEnumerable<T> UpdateObjects<T>(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (!await UpdateObject(item))
                    yield return item;
            }
        }

        /// <summary>
        /// Creates an object and inits the primary key field
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected virtual async Task<object> CreateObjectSetKey(Type item, object key)
        {
            var tinfo = await TypeParser.GetTypeInfo(item);
            return CreateObjectSetKey(item, key, tinfo);
        }

        /// <summary>
        /// Creates an object and inits the primary key field
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="key">The key.</param>
        /// <param name="ti">The type info.</param>
        /// <returns></returns>
        protected static object CreateObjectSetKey(Type item, object key, DatabaseTypeInfo ti)
        {
            object toReturn = item.GetConstructor([]).Invoke([]);
            ti.PrimaryKeys[0].Setter(toReturn, key);
            return toReturn;
        }

        /// <summary>
        /// This event will fire anytime an object is being loaded
        /// </summary>
        public event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;

        /// <summary>
        /// This event will fire just before an object is deleted
        /// </summary>
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;

        /// <summary>
        /// This event will fire just after an object is deleted
        /// </summary>
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;

        /// <summary>
        /// This event will fire just before an object is updated
        /// </summary>
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;

        /// <summary>
        /// This event will fire just after an object is updated
        /// </summary>
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;

        /// <summary>
        /// This event will fire just before an object is inserted
        /// </summary>
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserting;

        /// <summary>
        /// This event will fire just after an object is inserted
        /// </summary>
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        internal void FireObjectLoaded(object item)
        {
            ObjectLoaded?.Invoke(this, new ObjectInitializedEventArgs(item));
        }

        internal void FireObjectInserted(object item, bool result)
        {
            if (result)
                ObjectInserted?.Invoke(this, new ObjectInsertingEventArgs(item));
        }

        internal void FireObjectUpdated(object item, bool result)
        {
            if (result)
                ObjectUpdated?.Invoke(this, new ObjectUpdatingEventArgs(item));
        }

        internal void FireObjectDeleted(object item, bool result)
        {
            if (result)
                ObjectDeleted?.Invoke(this, new ObjectDeletingEventArgs(item));
        }

        internal bool CheckObjectUpdating(object item)
        {
            if (ObjectUpdating != null)
            {
                ObjectUpdatingEventArgs args = new ObjectUpdatingEventArgs(item);
                ObjectUpdating(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }

        internal bool CheckObjectDeleting(object item)
        {
            if (ObjectDeleting != null)
            {
                ObjectDeletingEventArgs args = new ObjectDeletingEventArgs(item);
                ObjectDeleting(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }

        internal bool CheckObjectInserting(object item)
        {
            if (ObjectInserting != null)
            {
                ObjectInsertingEventArgs args = new ObjectInsertingEventArgs(item);
                ObjectInserting(this, args);
                if (args.Cancel)
                    return false;
            }
            return true;
        }
    }
}
