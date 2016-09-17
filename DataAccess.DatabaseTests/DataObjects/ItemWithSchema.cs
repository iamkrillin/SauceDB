using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(Schema = "SauceTest")]
    public class ItemWithSchema
    {
        public int id { get; set; }
        public string Something { get; set; }
        public TimeSpan TimeSpent { get; set; }
    }

    [TableName(Schema = "SauceTest")]
    public class ItemWithSchemaAnotherColumn
    {
        public int id { get; set; }
        public string Something { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public string Name { get; set; }
    }
}
