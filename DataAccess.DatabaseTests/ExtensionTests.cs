using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void TestCanPage()
        {
            List<string> foo = new List<string>()
            {
                "foo", "bar", "foobar"
            };

            PageData<string> pagedata =  foo.AsQueryable().GetPage(1, 2);
            Assert.IsTrue(pagedata != null);
            Assert.IsTrue(pagedata.Data.Count() == 2);
            Assert.IsTrue(pagedata.NumPages == 2);
        }
    }
}
