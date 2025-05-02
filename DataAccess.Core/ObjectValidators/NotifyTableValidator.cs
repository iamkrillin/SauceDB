using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    public class NotifyTableValidator(IDataStore dstore) : ObjectValidator(dstore)
    {
        public override async Task ValidateObject(TypeParser tparser, DatabaseTypeInfo ti)
        {
            DBObject table = GetObject(ti);

            if (table == null)
                await CreateNewTable(ti);
            else
                await ValidateExistingTable(ti, table);
        }

        protected virtual Task CreateNewTable(DatabaseTypeInfo typeInfo)
        {
            throw new DataStoreException(string.Format("A Table {0} was requested but was not found in the datastore", typeInfo.TableName));
        }

        protected virtual Task ValidateExistingTable(DatabaseTypeInfo typeInfo, DBObject t)
        {
            List<Column> valid = [];

            foreach (DataFieldInfo dfi in typeInfo.DataFields)
            {
                Column c = t.Columns.Where(R => R.Name.Equals(dfi.FieldName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (c == null)
                {
                    throw new DataStoreException(string.Format("A column ({0}) was requested on table {1} by object type {2}, however, it is not present in the datastore", dfi.FieldName, typeInfo.TableName, typeInfo.DataType.FullName));
                }
                else
                {
                    valid.Add(new Column() { Name = dfi.FieldName });
                }
            }

            return Task.CompletedTask;
        }

        public override IEnumerable<DBObject> GetObjects()
        {
            if (Objects.Count < 1)
            {
                lock (Objects)
                {
                    Objects.AddRange(_dstore.Connection.GetSchemaTables(_dstore).ToBlockingEnumerable());
                }
            }
            return Objects;
        }

        public virtual Task CheckSchema(DatabaseTypeInfo ti)
        {
            return Task.CompletedTask;
        }
    }
}
