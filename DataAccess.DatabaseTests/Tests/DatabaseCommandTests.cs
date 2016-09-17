using DataAccess.Core;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests.Tests
{
    [TestClass]
    public abstract class DatabaseCommandTests
    {
        protected IDataStore dStore;
        public abstract IDataStore GetDataStore();

        public DatabaseCommandTests()
        {
            dStore = GetDataStore();
            Task.WaitAll(dStore.InitDataStore());
        }

        [TestMethod]
        public void CanDoDatabaseCommandWithDynamicObject()
        {
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });

            dynamic search = new ExpandoObject();
            search.Name = "Hello";

            IEnumerable<TestItemPrimaryKey> results = dStore.GetCommand<TestItemPrimaryKey>().ExecuteQuery("select * from TestItemPrimaryKeys where Name = @Name", search);
            Assert.IsNotNull(results);

            var list = results.ToList();
            Assert.IsTrue(list.Count() == 7);

        }
    }
}
