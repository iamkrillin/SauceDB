using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlCompact;

namespace DataAccess.DatabaseTests
{
    public class SqlServerCompactLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlCompactConnection.GetDataStore("C:\\Data.sdf");
        }

        public override void Test_Can_Do_Skip_Take()
        {
        }
    }
}
