using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemTwoKeys
    {
        [Key]
        public string Key1 { get; set; }

        [Key]
        public string Key2 { get; set; }

        public string OtherField { get; set; }
    }
}
