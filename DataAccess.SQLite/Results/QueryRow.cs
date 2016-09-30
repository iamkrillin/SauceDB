using DataAccess.Core.Data.Results;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.SQLite.Results
{
    public class QueryRow : IQueryRow
    {
        private ResultSet _result;
        public QueryRow NextRow { get; set; }
        public FieldData[] FieldData { get; set; }

        public QueryRow(ResultSet SetData, object [] data)
        {
            _result = SetData;
            FieldData = new FieldData[data.Length];
            for (int i = 0; i < data.Length; i++)
                FieldData[i] = new FieldData(data[i]);
        }

        public QueryRow(ResultSet SetData, FieldData[] data)
        {
            _result = SetData;
            FieldData = data;
        }

        public int? FindField(string field)
        {
            string search = field.ToUpper();
            int? result = null;
            if (FieldAvailable(search))
                result = _result.QueryFields[search];
            else
            {//check if there is a field in the form {name}_{marker}
                //This here accounts for how LINQ works, its not very uncommon to get the same field in a result set more than once,
                //if the field has been mapped once already for this row, try to find one that hasn't
                var key = _result.QueryFields.Keys.Where(r => r.StartsWith(search + "_") && !FieldData[_result.QueryFields[r]].Used).FirstOrDefault();
                if (key == null) 
                {//k, so that didn't work, just try to find something..
                    key = _result.QueryFields.Keys.Where(r => r.StartsWith(search + "_")).FirstOrDefault();
                }

                if(key != null)
                    result = _result.QueryFields[key];
            }

            return result;
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

        public void SetFieldData(object[] data)
        {
            FieldData = new FieldData[data.Length];
            for (int i = 0; i < data.Length; i++)
                FieldData[i] = new FieldData(data[i]);
        }

        public bool FieldAvailable(string key)
        {
            if (_result.QueryFields.ContainsKey(key))
                return !FieldData[_result.QueryFields[key]].Used;
            else
                return false;
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
            foreach (FieldData v in FieldData)
                v.Used = false;
        }
    }
}
