﻿using DataAccess.Core.Attributes;

namespace Tests.DataObjects
{
    public class TestItemAdditionalInit
    {
        public int id { get; set; }
        public string Something { get; set; }

        [IgnoredField]
        public int Calculated { get; set; }

        [AdditionalInit]
        public void Calculate()
        {
            Calculated = 15;
        }
    }
}
