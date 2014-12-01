using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(TableName = "TestItemThreeFields")]
    public class TestItemThreeFieldsPlusOneWithDefault
    {
        public string id { get; set; }
        public string something { get; set; }
        public string something2 { get; set; }

        [DataField(DefaultValue = "ADefault")]
        public string Something3 { get; set; }
    }
}
