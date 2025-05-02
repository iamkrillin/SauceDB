using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using MySql.Data.MySqlClient;
using Tests.DataObjects;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class MySQLSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore(ConnectionStrings.MYSQL);
        }

        [TestMethod]
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

        [TestMethod, ExpectedException(typeof(MySqlException))]
        public override async Task Test_Honors_Field_Length()
        {
            await base.Test_Honors_Field_Length();
            await dstore.InsertObject(new TestItemSmallString() { SmallString = "IAMHoweverToLongForTheFieldLength" });
        }

        [TestMethod]
        public async Task Test_MySql_Changes_String_Max_To_Text()
        {
            await dstore.InsertObject(new TestObjectMaxTextField() { Text = "Hello" });
            var objects = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(objects.ElementAt(0).Columns[1].DataType.Equals("longtext"));
        }
    }
}
