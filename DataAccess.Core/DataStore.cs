using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System.Collections;
using DataAccess.Core.Events;
using System.Reflection;
using DataAccess.Core.Linq;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core.Execute;
using DataAccess.Core.Schema;
using DataAccess.Core.ObjectFinders;
using DataAccess.Core.Data.Results;

namespace DataAccess.Core
{
    /// <summary>
    /// A data store
    /// </summary>
    public partial class DataStore : IDataStore
    {
        private IDataConnection _conn;

        /// <summary>
        /// Gets or sets the DataConnection
        /// </summary>
        /// <value></value>
        public IDataConnection Connection
        {
            get
            {
                return _conn;
            }
            set
            {
                _conn = value;
                _conn.CommandGenerator.DataStore = this;
            }
        }

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
        /// Gets or sets the type information parser.
        /// </summary>
        /// <value></value>
        public ITypeInformationParser TypeInformationParser { get; set; }

        /// <summary>
        /// Gets or sets the schema validator.
        /// </summary>
        /// <value>The schema validator.</value>
        public ISchemaValidator SchemaValidator { get; set; }

        /// <summary>
        /// Empty constructor, will not properly setup this class. to be used by sub classes only
        /// </summary>
        protected DataStore()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStore"/> class.
        /// </summary>
        /// <param name="Connection">The data connection</param>
        public DataStore(IDataConnection Connection)
        {
            this.Connection = Connection;
            this.ExecuteCommands = new ExecuteCommands();
            this.SchemaValidator = new ModifySchemaValidator(this);
            this.TypeInformationParser = new TypeParser(this);
            this.ObjectFinder = new SupportsSchemaObjectFinder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStore"/> class.
        /// </summary>
        /// <param name="Connection">The data connection.</param>
        /// <param name="ExecuteComamands">The command executor.</param>
        /// <param name="TypeParser">The type parser.</param>
        public DataStore(IDataConnection Connection, IExecuteDatabaseCommand ExecuteComamands, ITypeInformationParser TypeParser)
            : this(Connection)
        {
            this.ExecuteCommands = ExecuteComamands;
            this.TypeInformationParser = TypeParser;
        }

        /// <summary>
        /// Loads an object from the data store, the key must be set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool LoadObject(object item)
        {
            return LoadObject(item, false);
        }

        /// <summary>
        /// Loads an object from the data store, the key must be set
        /// </summary>
        /// <param name="item">The object to load</param>
        /// <param name="LoadAllFields">If true the loadfield=false will be ignored</param>
        /// <returns></returns>
        public virtual bool LoadObject(object item, bool LoadAllFields)
        {
            IDbCommand command = Connection.CommandGenerator.GetSelectCommand(item, LoadAllFields);
            return ProcessCommand(item, command, true);
        }

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <param name="item">The type to load</param>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        public virtual object LoadObject(Type item, object PrimaryKey)
        {
            return LoadObject(item, PrimaryKey, false);
        }

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="PrimaryKey">The primary key.</param>
        /// <returns></returns>
        public virtual T LoadObject<T>(object PrimaryKey)
        {
            return (T)LoadObject(typeof(T), PrimaryKey, false);
        }

        /// <summary>
        /// Loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="PrimaryKey">The primary key</param>
        /// <param name="LoadAllFields">if set to <c>true</c> [The load field attribute tag is ignored].</param>
        /// <returns></returns>
        public virtual T LoadObject<T>(object PrimaryKey, bool LoadAllFields)
        {
            return (T)LoadObject(typeof(T), PrimaryKey, LoadAllFields);
        }

        /// <summary>
        /// Loads an object.
        /// </summary>
        /// <param name="item">The type to load.</param>
        /// <param name="key">The primary field</param>
        /// <param name="LoadAllFields">if set to <c>true</c> [The load field attribute tag is ignored].</param>
        /// <returns></returns>
        public virtual object LoadObject(Type item, object key, bool LoadAllFields)
        {
            TypeInfo ti = TypeInformationParser.GetTypeInfo(item);
            if (ti.PrimaryKeys.Count == 1)
            {
                object toReturn = CreateObjectSetKey(item, key, ti);
                if (!LoadObject(toReturn, LoadAllFields)) toReturn = null;
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
        public virtual bool InsertObject(object item)
        {
            if (CheckObjectInserting(item))
            {
                IDbCommand command = Connection.CommandGenerator.GetInsertCommand(item);
                bool result = ProcessCommand(item, command);
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
        public virtual bool InsertObjects(IList items)
        {
            if (items.Count > 100)
            {
                Connection.DoBulkInsert(items, this);
                return true;
            }
            else
            {
                IDbCommand command = Connection.CommandGenerator.GetInsertCommand(items);
                return ProcessCommand(items, command);
            }
        }

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <param name="item">The type to delete</param>
        /// <param name="key">The primary key to delete on</param>
        /// <returns></returns>
        public virtual bool DeleteObject(Type item, object key)
        {
            return DeleteObject(CreateObjectSetKey(item, key));
        }

        /// <summary>
        /// Deletes an objet from the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <returns></returns>
        public virtual bool DeleteObject(object item)
        {
            if (CheckObjectDeleting(item))
            {
                IDbCommand command = Connection.CommandGenerator.GetDeleteCommand(item);
                bool result = ProcessCommand(item, command);
                FireObjectDeleted(item, result);

                return result;
            }
            else
                return false;
        }        

        /// <summary>
        /// Deletes an object
        /// </summary>
        /// <typeparam name="T">The type to delete</typeparam>
        /// <param name="primaryKey">The primary key to delete on></param>
        /// <returns></returns>
        public virtual bool DeleteObject<T>(object primaryKey)
        {
            return DeleteObject(typeof(T), primaryKey);
        }

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <param name="item">The type to load</param>
        /// <returns></returns>
        public virtual IEnumerable<object> LoadEntireTable(Type item)
        {
            return ExecuteCommandLoadList(item, Connection.CommandGenerator.LoadEntireTableCommand(item));
        }

        /// <summary>
        /// Loads an entire table
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> LoadEntireTable<T>()
        {
            return ExecuteCommandLoadList<T>(Connection.CommandGenerator.LoadEntireTableCommand(typeof(T)));
        }

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns></returns>
        public virtual string GetTableName(Type t)
        {
            return Connection.CommandGenerator.ResolveTableName(t);
        }

        /// <summary>
        /// Returns the resolved table name for a type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        public virtual string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        /// <summary>
        /// Returns the key value for an object
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="item">The object to extract from</param>
        /// <returns></returns>
        public virtual object GetKeyForItemType(Type type, object item)
        {
            IEnumerable<DataFieldInfo> fields = TypeInformationParser.GetPrimaryKeys(type);
            if (fields.Count() > 1) throw new DataStoreException("This Type contains more than one key");
            if (fields.Count() < 1) throw new DataStoreException("This type does not contain a key");
            return fields.ElementAt(0).Getter(item, null);
        }

        /// <summary>
        /// Determines if an object already exists in the data store, based on the primary key
        /// </summary>
        /// <param name="item">The object to check</param>
        /// <returns></returns>
        public virtual bool IsNew(object item)
        {
            IDbCommand cmd = Connection.CommandGenerator.GetSelectCommand(item, false);
            using (IQueryData qd = ExecuteCommands.ExecuteCommandQuery(cmd, Connection))
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
        public bool SaveObject(object item)
        {
            if (IsNew(item))
                return InsertObject(item);
            else
                return UpdateObject(item);
        }

        /// <summary>
        /// Updates an object on the data store, primary key must be set
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns></returns>
        public virtual bool UpdateObject(object item)
        {
            if (CheckObjectUpdating(item))
            {
                IDbCommand command = Connection.CommandGenerator.GetUpdateCommand(item);
                bool result = ProcessCommand(command, command);
                FireObjectUpdated(item, result);
                return result;
            }
            return false;
        }       

        /// <summary>
        /// Executes a command on the data store
        /// </summary>
        /// <param name="command">The command to execute</param>
        public virtual int ExecuteCommand(IDbCommand command)
        {
            return ExecuteCommands.ExecuteCommand(command, Connection);
        }

        /// <summary>
        /// Execute a command and loads an object
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual T ExecuteCommandLoadObject<T>(IDbCommand command)
        {
            return ExecuteCommandLoadList<T>(command).FirstOrDefault();
        }

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <param name="objectType">The type to load in the list</param>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual IEnumerable<object> ExecuteCommandLoadList(Type objectType, IDbCommand command)
        {
            return ExecuteCommandLoadList<object>(objectType, command);
        }

        /// <summary>
        /// Loads a list of objects from the data store
        /// </summary>
        /// <typeparam name="T">The type to load</typeparam>
        /// <param name="Ids">The primary key(s)</param>
        /// <returns></returns>
        public IEnumerable<T> LoadObjects<T>(IEnumerable Ids)
        {
            IDbCommand cmd = Connection.CommandGenerator.GetInCommand<T>(Ids);
            return ExecuteCommandLoadList<T>(cmd);
        }

        /// <summary>
        /// Executes a command and loads a list
        /// </summary>
        /// <typeparam name="T">The type to load in the list</typeparam>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual IEnumerable<T> ExecuteCommandLoadList<T>(IDbCommand command)
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
            TypeInfo ti = TypeInformationParser.GetTypeInfo(typeof(T));
            if (ti.QueryPredicate != null)
            {
                toReturn = ti.QueryPredicate.Invoke(toReturn);
            }

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
        /// Returns a comma separated list of the fields on an object
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns></returns>
        public virtual string GetSelectList<T>()
        {
            return Connection.CommandGenerator.GetSelectList(typeof(T), false);
        }

        /// <summary>
        /// Returns a comma separated list of the fields on an object
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns></returns>
        public virtual string GetSelectList(Type t)
        {
            return Connection.CommandGenerator.GetSelectList(t, false);
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns></returns>
        public virtual IQueryData ExecuteQuery(IDbCommand command)
        {
            return ExecuteCommands.ExecuteCommandQuery(command, Connection);
        }

        private object BuildObject(IQueryRow dt, TypeInfo ti)
        {
            object toReturn = ObjectBuilder.BuildObject(this, dt, ti);
            FireObjectLoaded(toReturn);
            return toReturn;
        }

        private void SetFieldData(TypeInfo typeInfo, IQueryRow dt, object p)
        {
            if (dt != null)
            {
                ObjectBuilder.SetFieldData(this, typeInfo, dt, p);
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
        protected virtual IEnumerable<ReturnType> ExecuteCommandLoadList<ReturnType>(Type objectType, IDbCommand command)
        {
            using (IQueryData dt = ExecuteCommands.ExecuteCommandQuery(command, Connection))
            {
                if (dt.QuerySuccessful)
                {
                    TypeInfo ti = TypeInformationParser.GetTypeInfo(objectType);
                    foreach (IQueryRow row in dt)
                    {
                        ReturnType toAdd;

                        if (objectType.IsSystemType())
                            toAdd = (ReturnType)Connection.CLRConverter.ConvertToType(row.GetDataForRowField(0), typeof(ReturnType));
                        else
                            toAdd = (ReturnType)BuildObject(row, ti);

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
        protected virtual bool ProcessCommand(object item, IDbCommand command)
        {
            return ProcessCommand(item, command, false);
        }

        /// <summary>
        /// Executes a db command and fills in a list of objects when done, if needed i.e. primary keys on insert etc
        /// </summary>
        /// <param name="item">The item being queried with</param>
        /// <param name="command">The db command</param>
        /// <returns></returns>
        protected virtual bool ProcessCommand(IList item, IDbCommand command)
        {
            return ProcessCommand(item, command, false);
        }

        /// <summary>
        /// Executes a db command and fills in a list of objects when done, if needed i.e. primary keys on insert etc
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="command">The db command</param>
        /// <param name="FailIfNoRecords">if set to <c>true</c> and there is no records in the result set, no further processing will be done</param>
        /// <returns></returns>
        protected virtual bool ProcessCommand(IList items, IDbCommand command, bool FailIfNoRecords)
        {
            return ProcessCommand(dt =>
            {
                if (items.Count > 0)
                {
                    Type t = items[0].GetType();
                    IEnumerator<IQueryRow> rows = dt.GetQueryEnumerator();

                    for (int i = 0; i < items.Count; i++)
                    {
                        rows.MoveNext();
                        SetFieldData(TypeInformationParser.GetTypeInfo(t), rows.Current, items[i]);
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
        protected virtual bool ProcessCommand(object item, IDbCommand command, bool FailIfNoRecords)
        {
            return ProcessCommand(r =>
            {
                var en = r.GetQueryEnumerator();
                if (en.MoveNext())
                {
                    SetFieldData(TypeInformationParser.GetTypeInfo(item.GetType()), en.Current, item);
                    return true;
                }
                else
                    return false;

            }, command, FailIfNoRecords);
        }

        protected virtual bool ProcessCommand(Func<IQueryData, bool> OnDone, IDbCommand command, bool FailIfNoRecords)
        {
            using (IQueryData dt = ExecuteCommands.ExecuteCommandQuery(command, Connection))
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
        public int DeleteObjects<T>(Expression<Func<T, bool>> criteria)
        {
            IDbCommand command = Connection.CommandGenerator.GetDeleteCommand<T>(criteria);
            return ExecuteCommands.ExecuteCommand(command, Connection);
        }

        /// <summary>
        /// Inits a transaction scope for the command executor
        /// </summary>
        /// <returns></returns>
        public virtual TransactionContext StartTransaction()
        {
            TransactionContext toReturn = new TransactionContext(this);

            //gotta marshal the events so they will propagate right from the transaction context
            toReturn.Instance.ObjectDeleted += (sender, args) => { if (ObjectDeleted != null) ObjectDeleted(sender, args); }; ;
            toReturn.Instance.ObjectDeleting += (sender, args) => { if (ObjectDeleting != null) ObjectDeleting(sender, args); };;
            toReturn.Instance.ObjectInserted += (sender, args) => { if (ObjectInserted != null) ObjectInserted(sender, args); };
            toReturn.Instance.ObjectInserting += (sender, args) => { if (ObjectInserting != null) ObjectInserting(sender, args); };;
            toReturn.Instance.ObjectLoaded += (sender, args) => { if (ObjectLoaded != null) ObjectLoaded(sender, args); };;
            toReturn.Instance.ObjectUpdated += (sender, args) => { if (ObjectUpdated != null) ObjectUpdated(sender, args); };
            toReturn.Instance.ObjectUpdating += (sender, args) => { if (ObjectUpdating != null) ObjectUpdating(sender, args); };

            return toReturn;
        }

        /// <summary>
        /// Returns a new instance of a data store on the same connection
        /// </summary>
        /// <returns></returns>
        public virtual IDataStore GetNewInstance()
        {
            IDataStore dstore = new DataStore(Connection, ExecuteCommands, TypeInformationParser);
            dstore.TypeInformationParser = new TypeParser(dstore);
            dstore.SchemaValidator = SchemaValidator;
            dstore.ExecuteCommands = ExecuteCommands;
            dstore.ObjectFinder = ObjectFinder;

            return dstore;
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
        public virtual IEnumerable<T> UpdateObjects<T>(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (!UpdateObject(item))
                {
                    yield return item;
                }
            }
        }        
    }
}
