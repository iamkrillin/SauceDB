using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.DatabaseTests.DataObjects
{
    [BypassValidation]
    public class TestItemBypassValidation
    {
        public int id { get; set; }
        public string Something { get; set; }
    }
}
