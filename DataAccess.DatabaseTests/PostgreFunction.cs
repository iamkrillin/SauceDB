using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using Xunit;
using DataAccess.DatabaseTests.DataObjects;

namespace DataAccess.DatabaseTests
{
    public class PostgreFunction : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore("Server=postgre;Port=5432;Database=Sauce;User Id=sauce;Password=Sauce;");
        }

        [Fact]
        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.True(dstore.Connection.LeftEscapeCharacter.Equals("\""));
            Assert.True(dstore.Connection.RightEscapeCharacter.Equals("\""));
        }


        [Fact]
        public override void Can_Do_Command_With_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems where \"Something\" = @query", new { query = "foo" });
            Assert.True(items != null);
            Assert.True(items.Count() == 1);
        }

        [Fact]
        public override void Can_Do_Command_Without_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems");
            Assert.True(items != null);
            Assert.True(items.Count() == 3);
        }

        [Fact]
        public override void Can_Do_Command_And_Set_Markers()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });
            var command = dstore.GetCommand<TestItem>();
            command.SetCommandText("select {{selectlist}} from {{tablename}} where \"Something\" = @query", "{{selectlist}}", "{{tablename}}");
            command.SetParameters(new { query = "foo" });

            var items = command.ExecuteCommandGetList();
            Assert.True(items != null);
            Assert.True(items.Count() == 1);
        }
    }
}
