using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemWithForeignKey
    {
        public int id { get; set; }
        public string Something { get; set; }

        [DataField(PrimaryKeyType = typeof(TestItem))]
        public int ForeignKey { get; set; }
    }
}
