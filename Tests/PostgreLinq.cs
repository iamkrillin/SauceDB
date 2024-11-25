using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.PostgreSQL;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class PostgreLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore(ConnectionStrings.POSTGRE);
        }

        public PostgreLinq()
            : base()
        {
            dStore = GetDataStore();
            dStore.InitDataStore().Wait();
        }
    }
}
