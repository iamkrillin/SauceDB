using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using Tests.DataObjects;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class PostgreFunction : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore(ConnectionStrings.POSTGRE);
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
