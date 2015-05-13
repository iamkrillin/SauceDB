using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using System.Data;
using DataAccess.SqlCompact;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerCompactSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            IDataStore toReturn = SqlCompactConnection.GetDataStore("C:\\Data.sdf");
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
