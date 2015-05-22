using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Data.Results
{
    public class QueryRow : IQueryRow
    {
        private IQueryData _result;
        public FieldData[] FieldData { get; set; }

        public QueryRow(IQueryData parent)
        {
            _result = parent;

        }

        public int? FindField(string field)
        {
            int? result = null;
            string search = field.ToUpper();

            if (FieldAvailable(search))
                result = _result.QueryFields[search];

            return result;
        }

        public bool FieldAvailable(string key)
        {
            if (_result.QueryFields == null) return false;
            return _result.QueryFields.ContainsKey(key);
        }

        public void SetFieldData(object[] data)
        {
            FieldData = new FieldData[data.Length];
            for (int i = 0; i < data.Length; i++)
                FieldData[i] = new FieldData(data[i]);
        }

        public object GetData(int field)
        {
            FieldData[field].Used = true;
            return FieldData[field].Data;
        }

        public object GetData(string field)
        {
            int? loc = FindField(field);
            if (loc.HasValue)
                return GetData(loc.Value);
            else
                return null;
        }

        public void ResetUsed()
        {
            if (FieldData != null)
            {
                foreach (FieldData v in FieldData)
                    v.Used = false;
            }
        }

        /// <summary>
        /// Indicates if a field contains a mapping
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool FieldHasMapping(string field)
        {
            return FindField(field).HasValue;
        }

        /// <summary>
        /// Returns the data for a given field and row
        /// </summary>
        /// <param name="field">The fields location</param>
        /// <param name="row">The row</param>
        /// <returns></returns>
        public object GetDataForRowField(int field)
        {
            return GetData(field);
        }

        /// <summary>
        /// Returns the data for a given field and row
        /// </summary>
        /// <param name="field">The fields name</param>
        /// <param name="row">The row index</param>
        /// <returns></returns>
        public object GetDataForRowField(string field)
        {
            return GetData(field);
        }
    }
}
