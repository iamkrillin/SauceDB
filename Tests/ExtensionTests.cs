using DataAccess.Core.Extensions;

namespace Tests
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

            PageData<string> pagedata = foo.AsQueryable().GetPage(1, 2);
            Assert.IsTrue(pagedata != null);
            Assert.IsTrue(pagedata.Data.Count() == 2);
            Assert.IsTrue(pagedata.NumPages == 2);
        }
    }
}
