using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;

namespace DataAccess.DatabaseTests
{
    public class SqlServerLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore("Data Source=sql;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;");
        }
    }
}
