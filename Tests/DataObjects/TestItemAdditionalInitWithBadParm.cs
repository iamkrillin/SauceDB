using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;
using DataAccess.Core.Interfaces;

namespace Tests.DataObjects
{
    public class TestItemAdditionalInitWithBadParm
    {
        public int id { get; set; }
        public string Something { get; set; }

        [IgnoredField]
        public int Calculated { get; set; }

        [AdditionalInit]
        public void Calculate(string foo)
        {
        }
    }
}
