using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemIgnoredField
    {
        public int id { get; set; }
        public string Something { get; set; }

        [IgnoredField]
        public string Ignored { get; set; }
    }
}
