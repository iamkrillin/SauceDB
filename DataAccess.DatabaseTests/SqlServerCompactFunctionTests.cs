using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Schema;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.SqlCompact;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerCompactFunctionTests : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlCompactConnection.GetDataStore(Path.GetTempFileName());
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.IsTrue(dstore.Connection.LeftEscapeCharacter.Equals("["));
            Assert.IsTrue(dstore.Connection.RightEscapeCharacter.Equals("]"));
        }

        [TestMethod]
        public void Test_TSql_Lower_Than_Min_Date_Returns_Null()
        {
            SqlCompactTypeConverter tConverter = new SqlCompactTypeConverter();
            object result = tConverter.ConvertToType<DateTime?>(DateTime.MinValue);
            Assert.IsNull(result);
        }
    }
}
