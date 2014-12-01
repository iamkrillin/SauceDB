using DataAccess.Core.Events;
using DataAccess.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Async
{
    public interface IAsyncDataStore
    {
        IDataStore UnderlyingStore { get; }
        ITypeInformationParser TypeInformationParser { get; }
        IExecuteDatabaseCommand ExecuteCommands { get; }
        IDataConnection Connection { get; }

        event EventHandler<ObjectInitializedEventArgs> ObjectLoaded;
        event EventHandler<ObjectDeletingEventArgs> ObjectDeleting;
        event EventHandler<ObjectDeletingEventArgs> ObjectDeleted;
        event EventHandler<ObjectUpdatingEventArgs> ObjectUpdating;
        event EventHandler<ObjectUpdatingEventArgs> ObjectUpdated;
        event EventHandler<ObjectInsertingEventArgs> ObjectInserting;
        event EventHandler<ObjectInsertingEventArgs> ObjectInserted;

        Task<bool> InsertObject(object item);
        Task<bool> InsertObjects(IList items);
        Task<object> LoadObject(Type item, object key, bool LoadAllFields = false);
        Task<bool> LoadObject(object item, bool LoadAllFields = false);
        Task<T> LoadObject<T>(object PrimaryKey, bool LoadAllFields = false);
        Task<bool> SaveObject(object item);
        Task<bool> IsNew(object item);
        Task<bool> UpdateObject(object item);
        Task<bool> DeleteObject(Type item, object key);
        Task<bool> DeleteObject(object item);
        Task<bool> DeleteObject<T>(object primaryKey);

        Task<IEnumerable<object>> LoadEntireTable(Type item);
        Task<IEnumerable<T>> LoadEntireTable<T>();
        Task<IEnumerable<object>> ExecuteCommandLoadList(Type objectType, IDbCommand command);
        Task<IEnumerable<T>> ExecuteCommandLoadList<T>(IDbCommand command);
        Task<IEnumerable<ReturnType>> ExecuteCommandLoadList<ReturnType>(Type objectType, IDbCommand command);

        Task<int> ExecuteCommand(IDbCommand command);
        Task<IEnumerable<T>> LoadObjects<T>(IEnumerable Ids);
        Task<IQueryData> ExecuteQuery(IDbCommand command);
        Task<int> DeleteObjects<T>(Expression<Func<T, bool>> criteria);
        Task<IQueryable<T>> Query<T>();
    }
}
