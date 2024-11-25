using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItem
    {
        public int id { get; set; }
        public string Something { get; set; }
        public TimeSpan TimeSpent { get; set; }
    }

    public class TestBulkItem
    {
        public string one { get; set; }
        public string two { get; set; }
        public string three { get; set; }
        public string four { get; set; }
    }
}
