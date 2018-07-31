using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class MySQLSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore("server=mysql;user id=test;password=test;persist security info=True;database=test");
        }

        [TestMethod]
        public override void Test_Can_Modify_Column_Type()
        {
            DatabaseTypeInfo ti1 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.ID = Guid.NewGuid().ToString();
            tipk.Date = "2010-11-20";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);


            DatabaseTypeInfo ti2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.IsTrue(ti2 != null);
        }

        [TestMethod, ExpectedException(typeof(MySqlException))]
        public override void Test_Honors_Field_Length()
        {
            base.Test_Honors_Field_Length();
            dstore.InsertObject(new TestItemSmallString() { SmallString = "IAMHoweverToLongForTheFieldLength" });
        }

        [TestMethod]
        public void Test_MySql_Changes_String_Max_To_Text()
        {
            dstore.InsertObject(new TestObjectMaxTextField() { Text = "Hello"});
            var objects = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(objects.ElementAt(0).Columns[1].DataType.Equals("longtext"));
        }
    }
}
