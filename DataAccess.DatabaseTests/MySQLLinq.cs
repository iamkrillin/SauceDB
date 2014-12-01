using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;

namespace DataAccess.DatabaseTests
{
    public class MySQLLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore("server=mysql;user id=test;password=test;persist security info=True;database=test");
        }
    }
}
