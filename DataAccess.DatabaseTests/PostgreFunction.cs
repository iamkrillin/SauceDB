using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public override void Can_Do_Command_With_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems where \"Something\" = @query", new { query = "foo" });
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 1);
        }

        [TestMethod]
        public override void Can_Do_Command_Without_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems");
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 3);
        }

        [TestMethod]
        public override void Can_Do_Command_And_Set_Markers()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });
            var command = dstore.GetCommand<TestItem>();
            command.SetCommandText("select {{selectlist}} from {{tablename}} where \"Something\" = @query", "{{selectlist}}", "{{tablename}}");
            command.SetParameters(new { query = "foo" });

            var items = command.ExecuteCommandGetList();
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 1);
        }
    }
}
