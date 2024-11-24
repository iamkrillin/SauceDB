﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.Core.Data;
using System.Threading.Tasks;

namespace DataAccess.Core.ObjectValidators
{
    /// <summary>
    /// Does view validation
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ViewValidator"/> class.
    /// </remarks>
    /// <param name="dstore">The dstore.</param>
    public class ViewValidator(IDataStore dstore) : ObjectValidator(dstore)
    {

        /// <summary>
        /// Validates an objects info against the datastore
        /// </summary>
        /// <param name="ti"></param>
        public override void ValidateObject(DatabaseTypeInfo ti)
        {
            DBObject obj = GetObject(ti);
            if (obj == null)
                CreateNewView(ti);
            else
                ValidateExistingView(ti, obj);
        }

        /// <summary>
        /// Validates an  existing view.
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <param name="obj">The obj.</param>
        protected virtual void ValidateExistingView(DatabaseTypeInfo ti, DBObject obj)
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
        }

        /// <summary>
        /// Creates the new view.
        /// </summary>
        /// <param name="ti">The ti.</param>
        /// <returns></returns>
        protected virtual DBObject CreateNewView(DatabaseTypeInfo ti)
        {
            if (ti.DataFields.Count > 0)
            {
                throw new DataStoreException(string.Format("The view requested by {0} ({1}) was not found", ti.DataType.Name, ti.TableName));
            }
            else
                return null;
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
                    Objects.AddRange(_dstore.Connection.GetSchemaViews(_dstore).ToBlockingEnumerable());
                }
            }

            return Objects;
        }
    }
}
