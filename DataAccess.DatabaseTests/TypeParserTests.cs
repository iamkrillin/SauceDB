using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests
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
        public void Test_FieldType_IsParsed_User_String()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItemDifferentFieldType));
            Assert.IsTrue(ti.DataFields[0].DataFieldType == Core.Attributes.FieldType.UserString);
            Assert.IsTrue(ti.DataFields[0].DataFieldString == "varchar(1000)");
        }

        [TestMethod]
        public void Test_Field_Lenth_Is_Parsed()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItemSmallString));
            Assert.IsTrue(ti.DataFields[1].FieldLength == 5);
        }

        [TestMethod]
        public void Test_Field_Length_Is_Null_When_Not_Specified()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItem));
            foreach (var v in ti.DataFields)
                Assert.IsTrue(!v.FieldLength.HasValue);
        }

        [TestMethod]
        public void Test_FieldType_Is_Default_When_Not_Specified()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItem));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.DataFields.Count == 3);
            foreach (var v in ti.DataFields)
                Assert.IsTrue(v.DataFieldType == Core.Attributes.FieldType.Default);
        }
    }
}
