using DataAccess.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DataAccess.Core.Data.Results
{
    /// <summary>
    /// Enumerates a query result set
    /// </summary>
    public class QueryEnumerator : IEnumerable<IQueryRow>, IEnumerator<IQueryRow>
    {
        private IDataReader reader;
        private QueryData _qd;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryEnumerator"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qd">The qd.</param>
        public QueryEnumerator(IDataReader reader, QueryData qd)
        {
            this.reader = reader;
            _qd = qd;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        public IQueryRow Current { get; private set; }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            reader.Dispose();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (reader.IsClosed) return false;

            if (reader.Read())
            {
                LoadRecord();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }
        }

        private void LoadRecord()
        {
            if (!_qd._recordLoaded)
                _qd.MapReturnData(reader);

            object[] values = new object[reader.FieldCount];
            reader.GetValues(values);
            Current = new QueryRow(_qd);
            Current.SetFieldData(values);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IQueryRow> GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
    }
}
