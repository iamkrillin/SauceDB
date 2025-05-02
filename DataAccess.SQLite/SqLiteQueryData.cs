using DataAccess.Core.Interfaces;
using DataAccess.SQLite.Results;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.SQLite
{
    /// <summary>
    /// Holds the query result data
    /// </summary>
    public class SqLiteQueryData : IEnumerable, IEnumerator, IQueryData
    {
        /// <summary>
        /// Indicates if the query was successful
        /// </summary>
        public bool QuerySuccessful { get; set; }

        /// <summary>
        /// The data set
        /// </summary>
        public ResultSet ResultData { get; set; }
        public int RowCount { get { return ResultData.Rows; } }

        public Dictionary<string, int> QueryFields
        {
            get { return ResultData.QueryFields; }
            set { ResultData.QueryFields = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SqLiteQueryData()
        {
            QuerySuccessful = false;
            ResultData = new ResultSet();
        }

        /// <summary>
        /// Add a field mapping
        /// </summary>
        /// <param name="field">The field name</param>
        /// <param name="location">The location the field is in the rows</param>
        public void AddFieldMapping(string field, int location)
        {
            ResultData.AddFieldMapping(field, location);
        }

        /// <summary>
        /// Retrieves the data mappings
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetMappings()
        {
            return ResultData.QueryFields;
        }

        /// <summary>
        /// Adds a row of data
        /// </summary>
        /// <param name="data"></param>
        public void AddRowData(object[] data)
        {
            ResultData.AddRow(data);
        }

        /// <summary>
        /// Indicates if a field contains a mapping
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool FieldHasMapping(string field)
        {
            if (ResultData.CurrentRow != null)
                return ResultData.CurrentRow.FindField(field).HasValue;
            else
                return false;
        }

        /// <summary>
        /// Returns the data for a given field and row
        /// </summary>
        /// <param name="field">The fields location</param>
        /// <param name="row">The row</param>
        /// <returns></returns>
        public object GetDataForRowField(int field)
        {
            return ResultData.CurrentRow.GetData(field);
        }

        /// <summary>
        /// Returns the data for a given field and row
        /// </summary>
        /// <param name="field">The fields name</param>
        /// <param name="row">The row index</param>
        /// <returns></returns>
        public object GetDataForRowField(string field)
        {
            if (ResultData.CurrentRow != null)
                return ResultData.CurrentRow.GetData(field);
            else
                return null;
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <returns></returns>
        public List<string> GetFields()
        {
            return ResultData.QueryFields.Keys.ToList();
        }

        public IEnumerator GetEnumerator()
        {
            return GetQueryEnumerator();
        }

        public object Current { get { return this; } }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            ResultData.NextRow();
            return ResultData.CurrentRow != null;
        }

        public void Reset()
        {
            ResultData.Reset();
        }

        internal void NextRow()
        {
            MoveNext();
        }

        public void ResetUsed()
        {
            ResultData.CurrentRow.ResetUsed();
        }

        public IEnumerator<IQueryRow> GetQueryEnumerator()
        {
            return new SqliteQueryEnumerator(this);
        }

        public void Dispose()
        {

        }
    }
}
