using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.Core;
using DataAccess.SqlServer;
using DataAccess.Core.Interfaces;
using System.IO;
using DataAccess.Core.Extensions;

namespace DataAccess.DatabaseTests
{
    public class ExtensionTests
    {
        [Fact]
        public void TestCanPage()
        {
            List<string> foo = new List<string>()
            {
                "foo", "bar", "foobar"
            };

            PageData<string> pagedata =  foo.AsQueryable().GetPage(1, 2);
            Assert.True(pagedata != null);
            Assert.True(pagedata.Data.Count() == 2);
            Assert.True(pagedata.NumPages == 2);
        }
    }
}
