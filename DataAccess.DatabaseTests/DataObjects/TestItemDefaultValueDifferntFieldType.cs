using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    [TableName(TableName = "TestItemDefaultValues")]
    public class TestItemDefaultValueDifferntFieldType
    {
        public int id { get; set; }

        [DataField(DefaultValue = "SomeDefaultValue", FieldTypeString = "varchar(1000)", FieldType=FieldType.UserString)]
        public string Something { get; set; }

        public string AnotherField { get; set; }
    }
}
