using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemDefaultValue
    {
        public int id { get; set; }

        [DataField(DefaultValue = "SomeDefaultValue")]
        public string Something { get; set; }

        public string AnotherField { get; set; }
    }
}
