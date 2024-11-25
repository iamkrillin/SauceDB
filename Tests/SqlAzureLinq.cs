using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqlAzureLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore(ConnectionStrings.SQL_SERVER);
        }
    }
}
