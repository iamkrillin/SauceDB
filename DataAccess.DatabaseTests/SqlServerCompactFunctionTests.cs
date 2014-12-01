using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using Xunit;
using DataAccess.Core.Schema;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.SqlCompact;

namespace DataAccess.DatabaseTests
{
    public class SqlServerCompactFunctionTests : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlCompactConnection.GetDataStore("C:\\Data.sdf");
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.True(dstore.Connection.LeftEscapeCharacter.Equals("["));
            Assert.True(dstore.Connection.RightEscapeCharacter.Equals("]"));
        }

        [Fact]
        public void Test_TSql_Lower_Than_Min_Date_Returns_Null()
        {
            SqlCompactTypeConverter tConverter = new SqlCompactTypeConverter();
            object result = tConverter.ConvertToType<DateTime?>(DateTime.MinValue);
            Assert.Null(result);
        }
    }
}
