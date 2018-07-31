using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core;
using System.Linq.Expressions;
using System.Collections;
using DataAccess.Core.Schema;

namespace DataAccess.DatabaseTests
{
    public class FakeDataStore : IDataStore, IDataConnection
    {
#pragma warning disable 0067
        public event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserting;
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        public IQueryData ExecuteQuery(IDbCommand command)
        {
            return new QueryData();
        }

        public IDataConnection Connection
        {
            get
            {
                return this;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IExecuteDatabaseCommand ExecuteCommands
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ISchemaValidator SchemaValidator
        {
            get
            {
                return new DoesNothingValidator();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool InsertObject(object item)
        {
            return true;
        }

        public object LoadObject(Type item, object PrimaryKey)
        {
            return new object();
        }

        public T LoadObject<T>(object PrimaryKey)
        {
            return default(T);
        }

        public object LoadObject(Type item, object key, bool LoadAllFields)
        {
            return new object();
        }

        public T LoadObject<T>(object PrimaryKey, bool LoadAllFields)
        {
            return default(T);
        }

        public T ExecuteCommandLoadObject<T>(System.Data.IDbCommand command)
        {
            return default(T);
        }

        public bool DeleteObject(Type item, object key)
        {
            return true;
        }

        public bool DeleteObject<T>(object primaryKey)
        {
            return true;
        }

        public IEnumerable<object> LoadEntireTable(Type item)
        {
            return new List<object>();
        }

        public IEnumerable<T> LoadEntireTable<T>()
        {
            return new List<T>();
        }

        public string GetTableName(Type t)
        {
            return "";
        }

        public string GetTableName<T>()
        {
            return "";
        }

        public IEnumerable<object> ExecuteCommandLoadList(Type objectType, System.Data.IDbCommand command)
        {
            return new List<object>();
        }

        public IEnumerable<T> ExecuteCommandLoadList<T>(System.Data.IDbCommand command)
        {
            return new List<T>();
        }

        public object GetKeyForItemType(Type type, object item)
        {
            return new object();
        }

        public bool LoadObject(object item)
        {
            return true;
        }

        public bool LoadObject(object item, bool LoadAllFields)
        {
            return true;
        }

        public bool InsertObjects(System.Collections.IList items)
        {
            return true;
        }

        public bool IsNew(object item)
        {
            return true;
        }

        public bool UpdateObject(object item)
        {
            return true;
        }

        public bool DeleteObject(object item)
        {
            return true;
        }

        public int ExecuteCommand(System.Data.IDbCommand command)
        {
            return 1;
        }

        public string GetSelectList(Type t)
        {
            return "";
        }

        public string GetSelectList<T>()
        {
            return "";
        }

        public IQueryable<T> Query<T>()
        {
            return null;
        }

        public SauceMapping GetQueryMapper()
        {
            return null;
        }

        public IConvertToCLR CLRConverter
        {
            get { return new StandardCLRConverter(); }
        }

        public ICommandGenerator CommandGenerator
        {
            get { return null; }
        }

        public IDbConnection GetConnection()
        {
            return null;
        }

        public IDbCommand GetCommand()
        {
            return null;
        }

        public IDbDataParameter GetParameter()
        {
            return null;
        }

        public IDbDataParameter GetParameter(string name, object value)
        {
            return null;
        }

        public string LeftEscapeCharacter
        {
            get { return null; }
        }

        public string RightEscapeCharacter
        {
            get { return null; }
        }

        public string DefaultSchema
        {
            get { return null; }
        }

        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            return null;
        }

        public void DoBulkInsert(System.Collections.IList items, IDataStore dstore)
        {
        }


        public int DeleteObjects<T>(Expression<Func<T, bool>> criteria)
        {
            return 1;
        }

        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            return null;
        }

        public IEnumerable<T> LoadObjects<T>(IEnumerable Ids)
        {
            return null;
        }


        public TransactionContext StartTransaction()
        {
            return null;
        }


        public IDataStore GetNewInstance()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            return new List<DBObject>();
        }

        public IEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            return new List<DBObject>();
        }


        public IFindDataObjects ObjectFinder
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool SaveObject(object item)
        {
            return true;
        }

        public IEnumerable<T> ExecuteCommandLoadList<T>(string command, object parameters)
        {
            return new List<T>();
        }


        public IEnumerable<T> ExecuteCommandLoadList<T>(string command, object parameters, CommandType type)
        {
            throw new NotImplementedException();
        }


        DatabaseCommand<T> IDataStore.GetCommand<T>()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<T> UpdateObjects<T>(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }


        public IConvertToDatastore DatastoreConverter
        {
            get { throw new NotImplementedException(); }
        }
    }
}
