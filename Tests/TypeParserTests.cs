using DataAccess.Core;
using DataAccess.Core.Attributes;
using DataAccess.Core.Data;
using Tests.DataObjects;


namespace Tests
{
    [TestClass]
    public class TypeParserTests
    {
        private TypeParser _parser;

        public TypeParserTests()
        {
            _parser = new TypeParser(new FakeDataStore());
        }

        [TestMethod]
        public async Task Test_FieldType_IsParsed_User_String()
        {
            DatabaseTypeInfo ti = await _parser.GetTypeInfo(typeof(TestItemDifferentFieldType));
            Assert.IsTrue(ti.DataFields[0].DataFieldType == FieldType.UserString);
            Assert.IsTrue(ti.DataFields[0].DataFieldString == "varchar(1000)");
        }

        [TestMethod]
        public async Task Test_Field_Lenth_Is_Parsed()
        {
            DatabaseTypeInfo ti = await _parser.GetTypeInfo(typeof(TestItemSmallString));
            Assert.IsTrue(ti.DataFields[1].FieldLength == 5);
        }

        [TestMethod]
        public async Task Test_Field_Length_Is_Null_When_Not_Specified()
        {
            DatabaseTypeInfo ti = await _parser.GetTypeInfo(typeof(TestItem));
            foreach (var v in ti.DataFields)
                Assert.IsTrue(!v.FieldLength.HasValue);
        }

        [TestMethod]
        public async Task Test_FieldType_Is_Default_When_Not_Specified()
        {
            DatabaseTypeInfo ti = await _parser.GetTypeInfo(typeof(TestItem));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.DataFields.Count == 3);
            foreach (var v in ti.DataFields)
                Assert.IsTrue(v.DataFieldType == FieldType.Default);
        }
    }
}
