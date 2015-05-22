using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlCompact;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerCompactLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlCompactConnection.GetDataStore(Path.GetTempFileName());
        }

        public override void Test_Can_Do_Skip_Take()
        {
        }
    }
}
