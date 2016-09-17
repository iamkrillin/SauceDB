using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core;
using DataAccess.SqlServer;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core.Data;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            IDataStore toReturn = SqlServerConnection.GetDataStore("Data Source=sql;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;");

            IDbCommand cmd = toReturn.Connection.GetCommand();
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
        public override async void Test_Honors_Field_Length()
        {
            base.Test_Honors_Field_Length();
            await dstore.InsertObject(new TestItemSmallString() { SmallString = "IAMHoweverToLongForTheFieldLength" });
        }

        [TestMethod]
        public virtual async Task Test_Sql_Changes_String_Max_To_varchar_max()
        {
            await dstore.InsertObject(new TestObjectMaxTextField() { Text = "Hello" });
            var objects = await dstore.SchemaValidator.TableValidator.GetObjects();
            var column = objects.ElementAt(0).Columns[1];

            Assert.IsTrue(column.DataType.Equals("varchar"));
            Assert.IsTrue(column.ColumnLength.Equals("MAX"));
        }
    }
}
