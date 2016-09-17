using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Events;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core;
using System.Linq.Expressions;
using System.Collections;
using DataAccess.Core.Schema;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests
{
    public class FakeDataStore : IDataStore, IDataConnection
    {
        public IConvertToCLR CLRConverter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IConvertToDatastore DatastoreConverter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICommandGenerator CommandGenerator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string LeftEscapeCharacter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string RightEscapeCharacter
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string DefaultSchema
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDataConnection Connection
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

        public ITypeInformationParser TypeInformationParser
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
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
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
#pragma warning disable 0067
        public event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserting;
        public event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;
        public event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;
        public event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        public IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        public IDbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        public IDbDataParameter GetParameter(string name, object value)
        {
            throw new NotImplementedException();
        }

        public Task<List<DBObject>> GetSchemaTables(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public Task<List<DBObject>> GetSchemaViews(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public IQueryProvider GetQueryProvider(IDataStore dStore)
        {
            throw new NotImplementedException();
        }

        public IDeleteFormatter GetDeleteFormatter(IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public void DoBulkInsert(IList items, IDataStore dstore)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryData> ExecuteQuery(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertObject(object item)
        {
            throw new NotImplementedException();
        }

        public Task<T> LoadObject<T>(object PrimaryKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteObject(object primaryKey)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteObjects<T>(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> LoadEntireTable(Type item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> LoadEntireTable<T>()
        {
            throw new NotImplementedException();
        }

        public string GetTableName(Type t)
        {
            throw new NotImplementedException();
        }

        public string GetTableName<T>()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> ExecuteCommandLoadList(Type objectType, IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> ExecuteCommandLoadList<T>(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteCommandLoadObject<T>(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public DatabaseCommand<T> GetCommand<T>()
        {
            throw new NotImplementedException();
        }

        public object GetKeyForItemType(Type type, object item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoadObject(object item)
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

        public Task<bool> SaveObject(object item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateObject(object item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> UpdateObjects<T>(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteCommand(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query<T>()
        {
            throw new NotImplementedException();
        }

        public SauceMapping GetQueryMapper()
        {
            throw new NotImplementedException();
        }

        public TransactionContext StartTransaction()
        {
            throw new NotImplementedException();
        }

        public IDataStore GetNewInstance()
        {
            throw new NotImplementedException();
        }
    }
}
