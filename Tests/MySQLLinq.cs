using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using DataAccess.Core.Data;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class MySQLLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore(ConnectionStrings.MYSQL);
        }
    }
}
