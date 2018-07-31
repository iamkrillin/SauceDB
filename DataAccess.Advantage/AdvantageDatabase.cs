﻿using Advantage.Data.Provider;
using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Events;
using DataAccess.Core.Execute;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Mapping;
using DataAccess.Core.ObjectFinders;
using DataAccess.Core.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageDatabase : DataStore
    {
        public AdvantageDatabase(string datapath, string tabletype, string servertype)      
            : base(new AdvantageConnection(datapath, tabletype, servertype))
        {
            SchemaValidator = new NotifyValidator(this);
            this.ExecuteCommands = new AdvantageCommandExecutor();
            this.ObjectFinder = new NoSchemaSupportObjectFinder();
        }

        public AdvantageDatabase(string connstring)
            : base(new AdvantageConnection(connstring))
        {
            SchemaValidator = new NotifyValidator(this);
            this.ExecuteCommands = new AdvantageCommandExecutor();
            this.ObjectFinder = new NoSchemaSupportObjectFinder();
        }

        public override TransactionContext StartTransaction()
        {
            return new AdvantageTransactionContext(this);
        }
    }
}
