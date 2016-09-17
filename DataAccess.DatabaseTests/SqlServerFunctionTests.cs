﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core;
using DataAccess.SqlServer;
using DataAccess.Core.Schema;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerFunctionTests : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore("Data Source=sql;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;");
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.IsTrue(dstore.Connection.LeftEscapeCharacter.Equals("["));
            Assert.IsTrue(dstore.Connection.RightEscapeCharacter.Equals("]"));
        }

        [TestMethod]
        public async Task Test_Transaction_Works_With_Temporary_Tables()
        {
            using (TransactionContext ctx = dstore.StartTransaction())
            {
                await ctx.Instance.GetCommand<int>().ExecuteCommand("create table #foo ( bar int )");
                await ctx.Instance.GetCommand<int>().ExecuteCommand("select * from #foo");
            }
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
