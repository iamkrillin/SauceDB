using DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemSmallString
    {
        public int ID { get; set; }

        [DataField(FieldLength = 5)]
        public string SmallString { get; set; }
    }
}
