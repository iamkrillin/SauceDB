using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
        public IDbConnection Connection { get; set; }
        
        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        public IDbTransaction Transaction { get; set; }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Transaction != null)
                Transaction.Dispose();

            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                    Connection.Close();

                Connection.Dispose();
            }
        }
    }
}
