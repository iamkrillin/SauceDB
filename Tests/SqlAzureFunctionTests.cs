using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.SqlServer.Types;
using Tests.DataObjects;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqlAzureFunctionTests : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore(ConnectionStrings.SQL_SERVER);
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.IsTrue(dstore.Connection.LeftEscapeCharacter.Equals("["));
            Assert.IsTrue(dstore.Connection.RightEscapeCharacter.Equals("]"));
        }

        [TestMethod]
        public void Test_TSql_Lower_Than_Min_Date_Returns_Null()
        {
            SqlServerTypeConverter tConverter = new SqlServerTypeConverter();
            object result = tConverter.ConvertToType<DateTime?>(DateTime.MinValue);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Test_Can_Insert_Spacial_Type()
        {
            Assert.IsTrue(await dstore.InsertObject(new TestItemWithGeography()
            {
                Location = SqlGeography.Point(1, 1, 4326)
            }));
        }

        [TestMethod]
        public async Task Test_Can_CRUD_Spacial_Type()
        {
            Assert.IsTrue(await dstore.InsertObject(new TestItemWithGeography()
            {
                Location = SqlGeography.Point(1, 1, 4326)
            }));

            TestItemWithGeography loaded = await dstore.LoadObject<TestItemWithGeography>(1);
            Assert.IsNotNull(loaded);
            Assert.IsNotNull(loaded.Location);
        }
    }
}
