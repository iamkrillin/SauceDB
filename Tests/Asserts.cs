using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public static class Asserts
    {
        public static void IsType(Type type, object data)
        {
            Assert.IsTrue(type == data.GetType());
        }
    }
}
