using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(TableName = "TestItemThreeFields")]
    public class TestItemThreeFieldsMinusOne
    {
        public string id { get; set; }
        public string something { get; set; }
    }
}
