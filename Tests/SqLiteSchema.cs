using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using Tests.DataObjects;
using Tests.Tests;

namespace Tests
{
    /// <summary>
    /// Summary description for SqLiteSchema
    /// </summary>
    [TestClass]
    public class SqLiteSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public override async Task Test_Can_RemoveColumn()
        {
            await base.Test_Can_RemoveColumn();
        }

        [TestMethod]
        public override Task Test_Honors_Field_Length()
        {//sqllite doesnt do this near as I can tell
            return Task.CompletedTask;
        }

        [TestMethod]
        public override async Task Test_Can_Get_Tables()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(tables.Count() > 0);
            foreach (DBObject t in tables)
            {
                if (t.Name.StartsWith("sqlite")) continue;
                Assert.IsTrue(t != null);
                Assert.IsTrue(t.Name != "");
                Assert.IsTrue(t.Schema != "");
                Assert.IsNotNull(t.Columns);
                Assert.IsTrue(t.Columns.Count > 0);

                foreach (Column c in t.Columns)
                {
                    Assert.IsTrue(c != null);
                    Assert.IsTrue(c.Name != "");
                }
            }
        }

        [TestMethod]
        public override async Task Test_DB_Types_Come_Back_Right()
        {
            dstore.LoadEntireTable<DBTypeTestObject>().ToBlockingEnumerable().ToList();
            IEnumerable<DBObject> objects = dstore.SchemaValidator.TableValidator.GetObjects(true);
            Assert.IsTrue(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("DBTypeTestObjects", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore = SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };

            dstore.LoadEntireTable<DBTypeTestObject>().ToBlockingEnumerable().ToList();
        }

        public override Task Test_Can_Modify_Column_Type()
        {
            return Task.CompletedTask;
        }
    }
}
