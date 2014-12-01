using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core.Data;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.DatabaseTests
{
    public class SqlAzureSchema : SchemaValidatorTests
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

        [Fact]
        public override void Test_Honors_Field_Length()
        {
            base.Test_Honors_Field_Length();

            Assert.Throws(typeof(SqlException), () =>
            {
                dstore.InsertObject(new TestItemSmallString() { SmallString = "IAMHoweverToLongForTheFieldLength" });
            });
        }

        [Fact]
        public void Test_Sql_Changes_String_Max_To_varchar_max()
        {
            dstore.InsertObject(new TestObjectMaxTextField() { Text = "Hello" });
            var objects = dstore.SchemaValidator.TableValidator.GetObjects();
            var column = objects.ElementAt(0).Columns[1];

            Assert.True(column.DataType.Equals("varchar"));
            Assert.True(column.ColumnLength.Equals("MAX"));
        }
    }
}
