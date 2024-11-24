using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class PostgreFunction : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore("Server=postgre;Port=5432;Database=Sauce;User Id=sauce;Password=Sauce;");
        }

        [TestMethod]
        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.IsTrue(dstore.Connection.LeftEscapeCharacter.Equals("\""));
            Assert.IsTrue(dstore.Connection.RightEscapeCharacter.Equals("\""));
        }


        [TestMethod]
        public override async Task Can_Do_Command_With_Parameter_Object()
        {
            await dstore.InsertObject(new TestItem() { Something = "foo" });
            await dstore.InsertObject(new TestItem() { Something = "bar" });
            await dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems where \"Something\" = @query", new { query = "foo" }).ToBlockingEnumerable();
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 1);
        }

        [TestMethod]
        public override async Task Can_Do_Command_Without_Parameter_Object()
        {
            await dstore.InsertObject(new TestItem() { Something = "foo" });
            await dstore.InsertObject(new TestItem() { Something = "bar" });
            await dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems").ToBlockingEnumerable();
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 3);
        }
    }
}
