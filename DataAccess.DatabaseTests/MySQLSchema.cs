using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using MySql.Data.MySqlClient;

namespace DataAccess.DatabaseTests
{
    public class MySQLSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore("server=mysql;user id=test;password=test;persist security info=True;database=test");
        }

        [Fact]
        public override void Test_Can_Modify_Column_Type()
        {
            TypeInfo ti1 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.True(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.ID = Guid.NewGuid().ToString();
            tipk.Date = "2010-11-20";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);


            TypeInfo ti2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.True(ti2 != null);
        }

        [Fact]
        public override void Test_Honors_Field_Length()
        {
            base.Test_Honors_Field_Length();

            Assert.Throws(typeof(MySqlException), () =>
            {
                dstore.InsertObject(new TestItemSmallString() { SmallString = "IAMHoweverToLongForTheFieldLength" });
            });
        }

        [Fact]
        public void Test_MySql_Changes_String_Max_To_Text()
        {
            dstore.InsertObject(new TestObjectMaxTextField() { Text = "Hello"});
            var objects = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.True(objects.ElementAt(0).Columns[1].DataType.Equals("longtext"));
        }
    }
}
