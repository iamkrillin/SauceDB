using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlCompact;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests
{
    [TestClass]
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
