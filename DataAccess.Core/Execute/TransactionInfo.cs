using System;
using System.Data;
using System.Data.Common;

namespace DataAccess.Core.Execute
{
    /// <summary>
    /// Information about a transaction
    /// </summary>
    public class TransactionInfo : IDisposable
    {
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>        
        public DbConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        public DbTransaction Transaction { get; set; }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Transaction?.Dispose();

            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                Connection.Dispose();
            }
        }
    }
}
