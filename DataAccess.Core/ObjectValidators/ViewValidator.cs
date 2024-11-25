using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Data;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    public class ViewValidator(IDataStore dstore) : ObjectValidator(dstore)
    {
        public override async Task ValidateObject(TypeParser tparser, DatabaseTypeInfo ti)
        {
            DBObject obj = GetObject(ti);
            if (obj == null)
                await CreateNewView(ti);
            else
                await ValidateExistingView(ti, obj);
        }

        protected virtual Task ValidateExistingView(DatabaseTypeInfo ti, DBObject obj)
        {
            List<string> Missing = [];

            foreach (DataFieldInfo dfi in ti.DataFields)
            {
                Column c = obj.Columns.Where(R => R.Name.Equals(dfi.FieldName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (c == null)
                    Missing.Add(dfi.FieldName);
            }

            if (Missing.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < Missing.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    sb.AppendFormat(" {0}", Missing[i]);
                }

                throw new DataStoreException(string.Format("The view {0} is missing the following columns {1}",  ti.TableName, sb.ToString()));
            }

            return Task.CompletedTask;
        }

        protected virtual Task<DBObject> CreateNewView(DatabaseTypeInfo ti)
        {
            if (ti.DataFields.Count > 0)
                throw new DataStoreException(string.Format("The view requested by {0} ({1}) was not found", ti.DataType.Name, ti.TableName));
            else
                return null;
        }

        public override IEnumerable<DBObject> GetObjects()
        {
            if (Objects.Count < 1)
            {
                lock (Objects)
                {
                    Objects.AddRange(_dstore.Connection.GetSchemaViews(_dstore).ToBlockingEnumerable());
                }
            }

            return Objects;
        }
    }
}
