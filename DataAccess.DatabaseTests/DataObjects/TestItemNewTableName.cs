using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(TableName = "SomeNewTable")]
    public class TestItemNewTableName
    {
        public int id { get; set; }
        public string Something { get; set; }
    }
}
