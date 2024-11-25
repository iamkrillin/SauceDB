using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using MySql.Data.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class MySqlFunction : FunctionTests
    {
        public class TestItemWithGeography
        {
            public int ID { get; set; }
            public MySqlGeometry Location { get; set; }
        }

        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore(ConnectionStrings.MYSQL);
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.IsTrue(dstore.Connection.LeftEscapeCharacter.Equals("`"));
            Assert.IsTrue(dstore.Connection.RightEscapeCharacter.Equals("`"));
        }

        [TestMethod]
        public async Task Test_Can_Insert_Spacial_Type()
        {
            Assert.IsTrue(await dstore.InsertObject(new TestItemWithGeography()
            {
                Location = new MySqlGeometry(1, 1, 4326)
            }));
        }

        [TestMethod]
        public async Task Test_Can_CRUD_Spacial_Type()
        {
            Assert.IsTrue(await dstore.InsertObject(new TestItemWithGeography()
            {
                Location = new MySqlGeometry(1, 1, 4326)
            }));

            TestItemWithGeography loaded = await dstore.LoadObject<TestItemWithGeography>(1);
            Assert.IsNotNull(loaded);
            Assert.IsNotNull(loaded.Location);
        }
    }
}
