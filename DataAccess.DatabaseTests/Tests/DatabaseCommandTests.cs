using DataAccess.Core.Interfaces;
using DataAccess.DatabaseTests.DataObjects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests.Tests
{
    public abstract class DatabaseCommandTests
    {
        protected IDataStore dStore;
        public abstract IDataStore GetDataStore();

        public DatabaseCommandTests()
        {
            dStore = GetDataStore();
            dStore.InitDataStore();
        }

        [Fact]
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
            Assert.NotNull(results);

            var list = results.ToList();
            Assert.True(list.Count() == 7);

        }
    }
}
