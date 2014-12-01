using DataAccess.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess.SQLite.Results
{
    /// <summary>
    /// Enumerates a query result set
    /// </summary>
    public class SqliteQueryEnumerator : IEnumerable<IQueryRow>, IEnumerator<IQueryRow>
    {
        private SqLiteQueryData _qd;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryEnumerator"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="qd">The qd.</param>
        public SqliteQueryEnumerator(SqLiteQueryData qd)
        {
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
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_qd.MoveNext())
            {
                LoadRecord();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoadRecord()
        {
            Current = new QueryRow(_qd.ResultData, _qd.ResultData.CurrentRow.FieldData);
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
