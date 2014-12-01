using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests
{
    public class TypeParserTests
    {
        private TypeParser _parser;
        
        public TypeParserTests()
        {
            _parser = new TypeParser(new FakeDataStore());
        }

        [Fact]
        public void Test_FieldType_IsParsed_User_String()
        {
            TypeInfo ti = _parser.GetTypeInfo(typeof(TestItemDefaultValueDifferntFieldType));
            Assert.True(ti.DataFields[1].DataFieldType == Core.Attributes.FieldType.UserString);
            Assert.True(ti.DataFields[1].DataFieldString == "varchar(1000)");
        }

        [Fact]
        public void Test_Field_Lenth_Is_Parsed()
        {
            TypeInfo ti = _parser.GetTypeInfo(typeof(TestItemSmallString));
            Assert.True(ti.DataFields[1].FieldLength == 5);
        }

        [Fact]
        public void Test_Field_Length_Is_Null_When_Not_Specified()
        {
            TypeInfo ti = _parser.GetTypeInfo(typeof(TestItem));
            foreach (var v in ti.DataFields)
                Assert.True(!v.FieldLength.HasValue);
        }

        [Fact]
        public void Test_FieldType_Is_Default_When_Not_Specified()
        {
            TypeInfo ti = _parser.GetTypeInfo(typeof(TestItem));
            Assert.NotNull(ti);
            Assert.True(ti.DataFields.Count == 3);
            foreach (var v in ti.DataFields)
                Assert.True(v.DataFieldType == Core.Attributes.FieldType.Default);
        }
    }
}
