using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemPrimaryKey
    {
        [Key(SetOnInsert = true)]
        public string ID { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    }

    [TableName(TableName = "TestItemPrimaryKeys")]
    public class TestItemPrimaryKeyDateFieldDifferentType
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
