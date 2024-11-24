﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    /// <summary>
    /// Validates tables on the datastore
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ModifyTableValidator"/> class.
    /// </remarks>
    /// <param name="dstore">The dstore.</param>
    public class NotifyTableValidator(IDataStore dstore) : ObjectValidator(dstore)
    {
        /// <summary>
        /// Validates an objects info against the datastore
        /// </summary>
        /// <param name="ti"></param>
        public override void ValidateObject(DatabaseTypeInfo ti)
        {
            DBObject table = GetObject(ti);

            if (table == null)
                CreateNewTable(ti);
            else
                ValidateExistingTable(ti, table);
        }

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        protected virtual void CreateNewTable(DatabaseTypeInfo typeInfo)
        {
            throw new DataStoreException(string.Format("A Table {0} was requested but was not found in the datastore", typeInfo.TableName));
        }

        /// <summary>
        /// Validates an existing table.
        /// </summary>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="t">The t.</param>
        protected virtual void ValidateExistingTable(DatabaseTypeInfo typeInfo, DBObject t)
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
        }

        /// <summary>
        /// Returns a list of objects from the datastore
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Does nothing...
        /// </summary>
        /// <param name="ti">The ti.</param>
        public virtual void CheckSchema(DatabaseTypeInfo ti)
        {
        }
    }
}
