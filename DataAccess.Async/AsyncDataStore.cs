using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;
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
    public partial class AsyncDataStore : IAsyncDataStore
    {
        public ITypeInformationParser TypeInformationParser { get { return _dstore.TypeInformationParser; } }
        public IExecuteDatabaseCommand ExecuteCommands { get { return _dstore.ExecuteCommands; } }
        public IDataConnection Connection { get { return _dstore.Connection; } }
        public IDataStore UnderlyingStore { get { return _dstore; } }
        private IDataStore _dstore;

        public AsyncDataStore(IDataStore dstore)
        {
            _dstore = dstore;
        }

        public async Task<bool> SaveObject(object item)
        {
            if (await IsNew(item))
                return await InsertObject(item);
            else
                return await UpdateObject(item);
        }

        public async Task<bool> IsNew(object item)
        {
            using (IQueryData qd = await Task.Run(() => {
                IDbCommand cmd = Connection.CommandGenerator.GetSelectCommand(item, false);
                return ExecuteCommands.ExecuteCommandQuery(cmd, Connection);
            }))
            {
                var enumerator = qd.GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current == null;
            }
        }

        public async Task<bool> UpdateObject(object item)
        {
            if (CheckObjectUpdating(item))
            {

                IDbCommand command = Connection.CommandGenerator.GetUpdateCommand(item);
                bool result = await ProcessCommandAsync(command, command);
                FireObjectUpdated(item, result);
                return result;
            }
            return false;
        }

        public async Task<T> LoadObject<T>(object PrimaryKey, bool LoadAllFields = false)
        {
            return (T)await LoadObject(typeof(T), PrimaryKey, LoadAllFields);
        }

        public async Task<object> LoadObject(Type item, object key, bool LoadAllFields = false)
        {
            TypeInfo ti = TypeInformationParser.GetTypeInfo(item);
            if (ti.PrimaryKeys.Count == 1)
            {
                object toReturn = CreateObjectSetKey(item, key, ti);
                if (!await LoadObject(toReturn, LoadAllFields)) 
                    toReturn = null;

                return toReturn;
            }
            else
            {
                throw new DataStoreException("This method is only valid when the item contains (1) key");
            }
        }

        public async Task<bool> LoadObject(object item, bool LoadAllFields = false)
        {
            IDbCommand command = Connection.CommandGenerator.GetSelectCommand(item, LoadAllFields);
            return await ProcessCommandAsync(item, command, true);
        }

        public async Task<bool> InsertObject(object item)
        {
            if (CheckObjectInserting(item))
            {
                IDbCommand command = _dstore.Connection.CommandGenerator.GetInsertCommand(item);
                bool result = await ProcessCommandAsync(item, command);
                Task.Run(() => FireObjectInserted(item, result));
                return result;
            }
            return false;
        }

        public async Task<bool> InsertObjects(IList items)
        {
            if (items.Count > 100)
            {
                return await Task.Run(() =>
                    {
                        Connection.DoBulkInsert(items, _dstore);
                        return true;
                    });
            }
            else
            {
                IDbCommand command = Connection.CommandGenerator.GetInsertCommand(items);
                return await ProcessCommandAsync(items, command);
            }
        }

        public async Task<bool> DeleteObject(Type item, object key)
        {
            return await DeleteObject(CreateObjectSetKey(item, key));
        }

        public async Task<bool> DeleteObject(object item)
        {
            if (CheckObjectDeleting(item))
            {

                IDbCommand command = Connection.CommandGenerator.GetDeleteCommand(item);
                bool result = await ProcessCommandAsync(item, command);
                FireObjectDeleted(item, result);

                return result;
            }
            else
                return false;
        }

        public async Task<bool> DeleteObject<T>(object primaryKey)
        {
            return await DeleteObject(typeof(T), primaryKey);
        }

        public async Task<IEnumerable<object>> LoadEntireTable(Type item)
        {
            return await ExecuteCommandLoadList(item, Connection.CommandGenerator.LoadEntireTableCommand(item));
        }

        public async Task<IEnumerable<T>> LoadEntireTable<T>()
        {
            return await ExecuteCommandLoadList<T>(Connection.CommandGenerator.LoadEntireTableCommand(typeof(T)));
        }

        public async Task<IEnumerable<T>> ExecuteCommandLoadList<T>(IDbCommand command)
        {
            return await ExecuteCommandLoadList<T>(typeof(T), command);
        }

        public async Task<IEnumerable<object>> ExecuteCommandLoadList(Type objectType, IDbCommand command)
        {
            return await ExecuteCommandLoadList<object>(objectType, command);
        }

        public async Task<IEnumerable<ReturnType>> ExecuteCommandLoadList<ReturnType>(Type objectType, IDbCommand command)
        {
            List<ReturnType> toReturn = new List<ReturnType>();

            using (IQueryData dt = await Task.Run(() => ExecuteCommands.ExecuteCommandQuery(command, Connection)))
            {
                if (dt.QuerySuccessful)
                {
                    TypeInfo ti = TypeInformationParser.GetTypeInfo(objectType);
                    foreach (IQueryRow row in dt)
                    {
                        if (objectType.IsSystemType())
                            toReturn.Add((ReturnType)Connection.CLRConverter.ConvertToType(row.GetDataForRowField(0), typeof(ReturnType)));
                        else
                            toReturn.Add((ReturnType) await BuildObjectAsync(row, ti));
                    }
                }
                return toReturn;
            }
        }

        public async Task<int> ExecuteCommand(IDbCommand command)
        {
            return await Task.Run(() => ExecuteCommands.ExecuteCommand(command, Connection));
        }

        public async Task<IEnumerable<T>> LoadObjects<T>(IEnumerable Ids)
        {
            return await Task.Run(() => _dstore.LoadObjects<T>(Ids));
        }

        public async Task<IQueryData> ExecuteQuery(IDbCommand command)
        {
            return await Task.Run(() => _dstore.ExecuteQuery(command));
        }

        public async Task<int> DeleteObjects<T>(Expression<Func<T, bool>> criteria)
        {
            return await Task.Run(() => _dstore.DeleteObjects<T>(criteria));
        }

        public async Task<IQueryable<T>> Query<T>()
        {
            var tTask = Task.Run(() => TypeInformationParser.GetTypeInfo(typeof(T)));
            var rTask = Task.Run(() => new Query<T>(Connection.GetQueryProvider(_dstore)));

            TypeInfo ti = await tTask;
            IQueryable<T> toReturn = await rTask;

            if (ti.QueryPredicate != null)
                toReturn = ti.QueryPredicate.Invoke(toReturn);

            return toReturn;
        }

        protected async Task<bool> ProcessCommandAsync(object item, IDbCommand command, bool FailIfNoRecords = false)
        {
            return await ProcessCommandAsync(r =>
            {
                var en = r.GetQueryEnumerator();
                if (en.MoveNext())
                {
                    var task = SetFieldDataAsync(TypeInformationParser.GetTypeInfo(item.GetType()), en.Current, item);
                    return true;
                }
                else
                    return false;

            }, command, FailIfNoRecords);
        }

        protected async Task<bool> ProcessCommandAsync(Func<IQueryData, bool> OnDone, IDbCommand command, bool FailIfNoRecords)
        {
            using (IQueryData dt = await Task.Run(() => _dstore.ExecuteCommands.ExecuteCommandQuery(command, _dstore.Connection)))
            {
                return await Task.Run(() =>
                {
                    if (dt.QuerySuccessful)
                    {
                        bool records = OnDone(dt);

                        if (FailIfNoRecords)
                            return records;
                    }
                    return true;
                });
            }
        }

        private async Task<object> BuildObjectAsync(IQueryRow dt, TypeInfo ti)
        {
            object toReturn = await Task.Run(() => ObjectBuilder.BuildObject(_dstore, dt, ti));
            FireObjectLoaded(toReturn);
            return toReturn;
        }

        private async Task SetFieldDataAsync(TypeInfo typeInfo, IQueryRow dt, object p)
        {
            if (dt != null)
            {
                await Task.Run(() => ObjectBuilder.SetFieldData(_dstore, typeInfo, dt, p));
                FireObjectLoaded(p);
            }
        }
    }
}
