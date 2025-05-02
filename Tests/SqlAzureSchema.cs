using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using Tests.DataObjects;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqlAzureSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            IDataStore toReturn = SqlServerConnection.GetDataStore(ConnectionStrings.SQL_SERVER);

            DbCommand cmd = toReturn.Connection.GetCommand();
            cmd.CommandText = "DROP SCHEMA NewSchema";

            try
            {
                toReturn.ExecuteCommand(cmd);
            }
            catch
            {// if the schema is already gone it will throw an exception... lets keep moving
            }


            return toReturn;
        }

        [TestMethod, ExpectedException(typeof(SqlException))]
        public override async Task Test_Honors_Field_Length()
        {
            await base.Test_Honors_Field_Length();
            await dstore.InsertObject(new TestItemSmallString() { SmallString = "IAMHoweverToLongForTheFieldLength" });
        }

        [TestMethod]
        public void Test_Sql_Changes_String_Max_To_varchar_max()
        {
            dstore.InsertObject(new TestObjectMaxTextField() { Text = "Hello" });
            var objects = dstore.SchemaValidator.TableValidator.GetObjects();
            var column = objects.ElementAt(0).Columns[1];

            Assert.IsTrue(column.DataType.Equals("varchar"));
            Assert.IsTrue(column.ColumnLength.Equals("MAX"));
        }
    }
}
