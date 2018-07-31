using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlAzureLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore("Data Source=172.16.0.3;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;");
        }
    }
}
