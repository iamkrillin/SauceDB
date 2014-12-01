using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemNullable
    {
        public int id { get; set; }
        public string Something { get; set; }
        public int? Item { get; set; }
        public TimeSpan TimeSpent { get; set; }
    }
}
