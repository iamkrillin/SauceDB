using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(TableName = "TestItemThreeFields")]
    public class TestItemThreeFieldsPlusOneForeign
    {
        public string id { get; set; }
        public string something { get; set; }
        public string something2 { get; set; }

        [DataField(PrimaryKeyType = typeof(TestItem))]
        public int Something3 { get; set; }
    }
}
