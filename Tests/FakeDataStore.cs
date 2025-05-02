using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core.Schema;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace Tests
{
    public class FakeDataStore : IDataStore, IDataConnection
    {
        public IDataConnection Connection => this;
        public IExecuteDatabaseCommand ExecuteCommands { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ISchemaValidator SchemaValidator { get => new DoesNothingValidator(); set { } }
        public IFindDataObjects ObjectFinder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TypeParser TypeParser => throw new NotImplementedException();
        public IConvertToCLR CLRConverter => throw new NotImplementedException();
        public IConvertToDatastore DatastoreConverter => throw new NotImplementedException();
        public ICommandGenerator CommandGenerator => throw new NotImplementedException();

        public string LeftEscapeCharacter => "";
        public string RightEscapeCharacter => "";
        public string DefaultSchema => "";

        public event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserting;
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        public Task<bool> DeleteObject(object item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteObject<T>(object pkey)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteObjects<T>(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public Task DoBulkInsert(IList items, IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteCommand(DbCommand command)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object> ExecuteCommandLoadList(Type objectType, DbCommand command)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> ExecuteCommandLoadList<T>(DbCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteCommandLoadObject<T>(DbCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryData> ExecuteQuery(DbCommand command)
        {
            throw new NotImplementedException();
        }

        public DatabaseCommand<T> GetCommand<T>()
        {
            throw new NotImplementedException();
        }

        public DbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        public DbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetKeyForItemType(Type type, object item)
        {
            throw new NotImplementedException();
        }

        public IDataStore GetNewInstance()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter GetParameter(string name, object value)
        {
            throw new NotImplementedException();
        }

        public SauceMapping GetQueryMapper()
        {
            throw new NotImplementedException();
        }

        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<DBObject> GetSchemaTables(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<DBObject> GetSchemaViews(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTableName(Type t)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTableName<T>()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertObject(object item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertObjects(IList items)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsNew(object item)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object> LoadEntireTable(Type item)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> LoadEntireTable<T>()
        {
            throw new NotImplementedException();
        }

        public Task<T> LoadObject<T>(object PrimaryKey)
        {
            throw new NotImplementedException();
        }

        public Task<object> LoadObject(Type dtoType, object PrimaryKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoadObject(object item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query<T>()
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveObject(object item)
        {
            throw new NotImplementedException();
        }

        public TransactionContext StartTransaction()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateObject(object item)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> UpdateObjects<T>(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }
    }
}
