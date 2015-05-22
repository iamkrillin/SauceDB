using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.SqlCompact;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerCompactSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            IDataStore toReturn = SqlCompactConnection.GetDataStore(Path.GetTempFileName());
            IDbCommand cmd = toReturn.Connection.GetCommand();
            return toReturn;
        }

        [TestMethod]
        public override void Test_Honors_Field_Length()
        {
            base.Test_Honors_Field_Length();
        }

        [TestMethod]
        public override void Test_Can_Modify_Column_And_Keep_Default_Value()
        {
        }
    }
}
