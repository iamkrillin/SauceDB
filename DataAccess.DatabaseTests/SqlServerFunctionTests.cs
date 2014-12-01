using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Xunit;
using DataAccess.Core.Schema;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.SqlServer.Types;

namespace DataAccess.DatabaseTests
{
    public class SqlServerFunctionTests : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore("Data Source=sql;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;");
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.True(dstore.Connection.LeftEscapeCharacter.Equals("["));
            Assert.True(dstore.Connection.RightEscapeCharacter.Equals("]"));
        }

        [Fact]
        public void Test_TSql_Lower_Than_Min_Date_Returns_Null()
        {
            SqlServerTypeConverter tConverter = new SqlServerTypeConverter();
            object result = tConverter.ConvertToType<DateTime?>(DateTime.MinValue);
            Assert.Null(result);
        }

        [Fact]
        public void Test_Can_Insert_Spacial_Type()
        {
            Assert.True(dstore.InsertObject(new TestItemWithGeography() 
            {
                Location = SqlGeography.Point(1, 1, 4326)
            }));
        }

        [Fact]
        public void Test_Can_CRUD_Spacial_Type()
        {
            Assert.True(dstore.InsertObject(new TestItemWithGeography()
            {
                Location = SqlGeography.Point(1, 1, 4326)
            }));

            TestItemWithGeography loaded = dstore.LoadObject<TestItemWithGeography>(1);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.Location);
        }
    }
}
