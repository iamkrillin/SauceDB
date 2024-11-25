using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemWithKeyAttribute
    {
        [Key]
        public string Something { get; set; }
    }
}
