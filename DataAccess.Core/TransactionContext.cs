using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Execute;

namespace DataAccess.Core
{
    /// <summary>
    /// Context to use for transactions
    /// </summary>
    public class TransactionContext : IDisposable
    {
        private IDataStore _parent;
        /// <summary>
        /// Operations ran here will happen in the context of a transaction
        /// </summary>
        public IDataStore Instance { get; set; }

        /// <summary>
        /// Information about the transaction
        /// </summary>
        public TransactionInfo TransactionInfo { get; set; }

        /// <summary>
        /// Fires during the dispose event
        /// </summary>
        public event EventHandler OnDisposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionContext" /> class.
        /// </summary>
        /// <param name="store">The datastore to use.</param>
        public TransactionContext(IDataStore store)
        {
            _parent = store;
            Instance = store.GetNewInstance();
            StartNewTransaction();
        }

        /// <summary>
        /// Rollbacks a transaction
        /// </summary>
        public void Rollback()
        {
            if (TransactionInfo != null)
            {
                TransactionInfo.Transaction.Rollback();
                TransactionInfo.Dispose();
                TransactionInfo = null;
            }
        }

        /// <summary>
        /// Commits a transaction
        /// </summary>
        public void Commit()
        {
            if (TransactionInfo != null)
            {
                TransactionInfo.Transaction.Commit();
                TransactionInfo.Dispose();
                TransactionInfo = null;
            }
        }

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        public virtual void StartNewTransaction()
        {
            TransactionInfo = new TransactionInfo();
            OpenConnection();
            TransactionInfo.Transaction = TransactionInfo.Connection.BeginTransaction();
            Instance.ExecuteCommands = new TransactionCommandExecutor(TransactionInfo, Instance.ExecuteCommands);
        }

        /// <summary>
        /// Opens the sql connection
        /// </summary>
        public virtual void OpenConnection()
        {
            TransactionInfo.Connection = Instance.Connection.GetConnection();
            TransactionInfo.Connection.Open();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (TransactionInfo != null)
                Rollback();

            if (OnDisposing != null)
                OnDisposing(this, new EventArgs());
        }
    }
}
