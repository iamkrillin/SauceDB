using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using Tests.DataObjects;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class PostgreSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore(ConnectionStrings.POSTGRE);
        }

        public PostgreSchema()
        {
            dstore = GetDataStore();
            dstore.InitDataStore().Wait();
        }

        [TestMethod]
        public override async Task Test_Can_Get_Tables()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(tables.Count() > 0);
            foreach (DBObject t in tables)
            {
                Assert.IsTrue(t != null);
                Assert.IsTrue(t.Name != "");
                Assert.IsNotNull(t.Columns);
                Assert.IsTrue(t.Columns.Count > 0);

                foreach (Column c in t.Columns)
                {
                    Assert.IsTrue(c != null);
                    Assert.IsTrue(c.Name != "");
                }
            }
        }

        public override async Task Test_Can_Modify_Column_Type()
        {
            DatabaseTypeInfo ti1 = await dstore.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.ID = Guid.NewGuid().ToString();
            tipk.Date = "2010-11-20";
            tipk.Name = "Hello";
            await dstore.InsertObject(tipk);


            DatabaseTypeInfo ti2 = await dstore.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.IsTrue(ti2 != null);
        }
    }
}
