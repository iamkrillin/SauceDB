using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;
using DataAccess.Core.Interfaces;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemOnTableCreated
    {
        public TestItemOnTableCreated()
        {

        }

        public int id { get; set; }
        public string Name { get; set; }

        [OnTableCreate]
        private static void AddDefaultValues(IDataStore dStore)
        {
            dStore.InsertObject(new TestItemOnTableCreated() { Name = ">" });
            dStore.InsertObject(new TestItemOnTableCreated() { Name = ">=" });
            dStore.InsertObject(new TestItemOnTableCreated() { Name = "<" });
            dStore.InsertObject(new TestItemOnTableCreated() { Name = "<=" });
            dStore.InsertObject(new TestItemOnTableCreated() { Name = "==" });
            dStore.InsertObject(new TestItemOnTableCreated() { Name = "!=" });
        }
    }

    public class TestItemOnTableCreatedNonStatic
    {
        public string Foo { get; set; }

        [OnTableCreate]
        private void AddDefaultValues(IDataStore dStore)
        {
        }
    }
}
