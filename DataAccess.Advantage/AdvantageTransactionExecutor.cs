using Advantage.Data.Provider;
using DataAccess.Core.Execute;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageTransactionExecutor : TransactionCommandExecutor
    {
        public AdvantageTransactionExecutor(TransactionInfo info, IExecuteDatabaseCommand storeExecutor)
            : base(info, storeExecutor)
        {

        }
    }
}
