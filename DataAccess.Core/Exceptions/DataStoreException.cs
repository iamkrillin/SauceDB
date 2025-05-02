using System;

namespace DataAccess.Core
{
    /// <summary>
    /// Represents a data store error
    /// </summary>
    public class DataStoreException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStoreException"/> class.
        /// </summary>
        /// <param name="msg">The message.</param>
        public DataStoreException(string msg)
            : base(msg)
        {

        }
    }
}
