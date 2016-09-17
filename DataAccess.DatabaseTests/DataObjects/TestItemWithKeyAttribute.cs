using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemWithKeyAttribute
    {
        [Key]
        public string Something { get; set; }
    }
}
