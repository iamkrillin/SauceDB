using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests.DataObjects
{
    public class DBTypeTestObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public byte[] Data { get; set; }
        public TimeSpan LengthOfTime { get; set; }
        public bool IsOn { get; set; }
        public double Amount { get; set; }
        public long AmountTwo { get; set; }
        public char Initial { get; set; }
    }
}
