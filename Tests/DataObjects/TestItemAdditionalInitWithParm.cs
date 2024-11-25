using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;
using DataAccess.Core.Interfaces;

namespace Tests.DataObjects
{
    public class TestItemAdditionalInitWithParm
    {
        public int id { get; set; }
        public string Something { get; set; }

        [IgnoredField]
        public int Calculated { get; set; }

        [AdditionalInit]
        private void Calculate(IDataStore dStore)
        {
            if (dStore == null)
            {
                throw new Exception("Booger");
            }
            else
            {
                dStore.InsertObject(new TestItemAdditionalInit() { Something = "Hello" });
                Calculated = Something.Length;
            }
        }
    }
}
