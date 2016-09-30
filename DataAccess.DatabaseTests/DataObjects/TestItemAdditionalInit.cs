using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class TestItemAdditionalInit
    {
        public int id { get; set; }
        public string Something { get; set; }

        [IgnoredField]
        public int Calculated { get; set; }

        [AdditionalInitAttribute]
        public void Calculate()
        {
            Calculated = 15;
        }
    }
}
