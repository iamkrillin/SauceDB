using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using Xunit;
using MySql.Data.Types;

namespace DataAccess.DatabaseTests
{
    public class MySqlFunction : FunctionTests
    {
        public class TestItemWithGeography
        {
            public int ID { get; set; }
            public MySqlGeometry Location { get; set; }
        }

        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore("server=mysql;user id=test;password=test;persist security info=True;database=test");
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.True(dstore.Connection.LeftEscapeCharacter.Equals("`"));
            Assert.True(dstore.Connection.RightEscapeCharacter.Equals("`"));
        }

        [Fact]
        public void Test_Can_Insert_Spacial_Type()
        {
            Assert.True(dstore.InsertObject(new TestItemWithGeography()
            {
                Location = new MySqlGeometry(1, 1, 4326)
            }));
        }

        [Fact]
        public void Test_Can_CRUD_Spacial_Type()
        {
            Assert.True(dstore.InsertObject(new TestItemWithGeography()
            {
                Location = new MySqlGeometry(1, 1, 4326)
            }));

            TestItemWithGeography loaded = dstore.LoadObject<TestItemWithGeography>(1);
            Assert.NotNull(loaded);
            Assert.NotNull(loaded.Location);
        }
    }
}
