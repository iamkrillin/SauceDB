using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestObjectManyColumns
    {
        public int ID { get; set; }
        public int NumberColumns { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan Span { get; set; }
        public bool IsOn { get; set; }
        public long Foo { get; set; }
    }
}
