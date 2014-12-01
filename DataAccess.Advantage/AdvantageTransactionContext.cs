using Advantage.Data.Provider;
using DataAccess.Core;
using DataAccess.Core.Execute;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageTransactionContext : TransactionContext
    {
        public AdvantageTransactionContext(IDataStore dstore)
            : base(dstore)
        {

        }

        public override void StartNewTransaction()
        {
            TransactionInfo = new TransactionInfo();
            TransactionInfo.Connection = Instance.Connection.GetConnection();
            TransactionInfo.Connection.Open();
            TransactionInfo.Transaction = TransactionInfo.Connection.BeginTransaction();
            Instance.ExecuteCommands = new AdvantageTransactionExecutor(TransactionInfo, Instance.ExecuteCommands);
        }

        public override void OpenConnection()
        {
            TransactionInfo.Connection = Instance.Connection.GetConnection();
            TransactionInfo.Connection.Open();
            ((AdsConnection)TransactionInfo.Connection).DateFormat = "YYYY-MM-DD";
        }
    }
}
