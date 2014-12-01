using DataAccess.Core;
using DataAccess.DatabaseTests.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests
{
    public class StandardCLRConverterTests
    {
        [Fact]
        public virtual void Test_Can_Convert_DBNull()
        {
            DBNull toConvert = DBNull.Value;
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(toConvert, typeof(bool));
            Assert.Null(result);
        }

        [Fact]
        public virtual void Test_Can_Convert_True_To_Boolean()
        {
            List<string> toTest = new List<string>()
              {
                "true",
                "True",
                "TRUE",
                "TrUe",
                "TRue",
                "T",
                "t",
                "1"
              };

            foreach (string s in toTest)
            {
                StandardCLRConverter tConverter = new StandardCLRConverter();
                object result = tConverter.ConvertToType(s, typeof(bool));
                Assert.NotNull(result);
                Assert.IsType(typeof(bool), result);
                bool bResult = (bool)result;
                Assert.True(bResult);
            }
        }

        [Fact]
        public virtual void Test_Can_Convert_Nullable()
        {

            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(1, typeof(Data?));
            Assert.NotNull(result);
            Assert.True(((Data?)result).Value == Data.Var1);
        }

        [Fact]
        public virtual void Test_Can_Convert_Guid_To_String()
        {
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(Guid.NewGuid(), typeof(Guid));
            Assert.NotNull(result);
            Assert.IsType(typeof(string), result);
            string bResult = (string)result;
            Assert.True(!string.IsNullOrEmpty(bResult));
        }

        [Fact]
        public virtual void Test_Can_Parse_DateTimeOffset()
        {
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(DateTimeOffset.Now.ToString(), typeof(DateTimeOffset));
            Assert.NotNull(result);
            Assert.IsType(typeof(DateTimeOffset), result);
        }

        [Fact]
        public virtual void Test_Can_Convert_DateTime()
        {
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(DateTime.Now.ToString(), typeof(DateTime));
            Assert.NotNull(result);
            Assert.IsType(typeof(DateTime), result);
        }

        [Fact]
        public virtual void Test_Can_Convert_Int()
        {
            List<object> toTest = new List<object>()
              {
                "1234",
                1,
                1234,
                int.MaxValue,
                int.MinValue
              };

            foreach (object s in toTest)
            {
                StandardCLRConverter tConverter = new StandardCLRConverter();
                object result = tConverter.ConvertToType(s, typeof(int));
                Assert.NotNull(result);
                Assert.IsType(typeof(int), result);
            }
        }

        [Fact]
        public virtual void Test_Can_Convert_Double()
        {
            List<object> toTest = new List<object>()
              {
                "1234.1234",
                1.234,
                123.4,
                double.MaxValue,
                int.MinValue
              };

            foreach (object s in toTest)
            {
                StandardCLRConverter tConverter = new StandardCLRConverter();
                object result = tConverter.ConvertToType(s, typeof(double));
                Assert.NotNull(result);
                Assert.IsType(typeof(double), result);
            }
        }

        [Fact]
        public virtual void Test_Can_Convert_Using_Template()
        {
            string item = "123.12";

            StandardCLRConverter tConverter = new StandardCLRConverter();
            double result = tConverter.ConvertToType<double>(item);
            Assert.True(result == 123.12);
        }
    }
}
