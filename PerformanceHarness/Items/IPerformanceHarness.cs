using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PerformanceHarness.Items
{
    public interface IPerformanceHarness
    {
        void InsertTestObject(TestClass item);
        void CleanUp();
        string Name { get; }

        TestClass ReadObject(int p);
    }
}
