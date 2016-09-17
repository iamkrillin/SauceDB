using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;
using DataAccess.Core.Data.Results;
using System.Collections;
using System.Data;

namespace DataAccess.Core.Data
{
    /// <summary>
    /// Holds the query result data
    /// </summary>
    public class QueryData : IDisposable, IEnumerable, IQueryData
    {
        public bool _recordLoaded = false;
        protected IDbConnection connection;
        protected IDbCommand command;
        protected IDataReader reader;
        public Dictionary<string, int> QueryFields { get; set; }

        /// <summary>
        /// Indicates if the query was successful
        /// </summary>
        public bool QuerySuccessful { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryData()
        {
            QuerySuccessful = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            CloseConnection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryData"/> class.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="command">The command.</param>
        public QueryData(System.Data.IDbConnection r, System.Data.IDbCommand command)
            : this()
        {
            this.connection = r;
            this.command = command;
            QuerySuccessful = true;
        }

        /// <summary>
        /// Maps return data to the query data
        /// </summary>
        /// <param name="reader">The reader.</param>
        public virtual void MapReturnData(IDataReader reader)
        {
            QueryFields = new Dictionary<string, int>();
            
            int len = reader.FieldCount;
            for (int i = 0; i < len; i++)
                AddFieldMapping(reader.GetName(i), i);
        }

        protected virtual void CloseConnection()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }

            if (connection != null && command.Transaction == null)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                connection.Dispose();
                connection = null;
                reader = null;
            }
        }

        /// <summary>
        /// Add a field mapping
        /// </summary>
        /// <param name="field">The field name</param>
        /// <param name="location">The location the field is in the rows</param>
        public void AddFieldMapping(string field, int location)
        {
            if (QueryFields == null)
                QueryFields = new Dictionary<string, int>();

            field = field.Split('.').Last().Replace("\"", "").ToUpper();
            if (!QueryFields.ContainsKey(field))
                QueryFields.Add(field, location);
        }

        /// <summary>
        /// Retrieves the data mappings
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetMappings()
        {
            return QueryFields;
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <returns></returns>
        public List<string> GetFields()
        {
            return QueryFields.Keys.ToList();
        }

        public IEnumerator<IQueryRow> GetQueryEnumerator()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }

            if (connection.State != ConnectionState.Open)
                connection.Open();

            reader = command.ExecuteReader();
            return new QueryEnumerator(reader, this);
        }

        public IEnumerator GetEnumerator()
        {
            return GetQueryEnumerator();
        }
    }
}
