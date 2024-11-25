using DataAccess.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tests.DataObjects;

namespace Tests
{
    [TestClass]
    public class StandardCLRConverterTests
    {
        [TestMethod]
        public virtual void Test_Can_Convert_DBNull()
        {
            DBNull toConvert = DBNull.Value;
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(toConvert, typeof(bool));
            Assert.IsNull(result);
        }

        [TestMethod]
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
                Assert.IsNotNull(result);
                Asserts.IsType(typeof(bool), result);
                bool bResult = (bool)result;
                Assert.IsTrue(bResult);
            }
        }

        [TestMethod]
        public virtual void Test_Can_Convert_Nullable()
        {

            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(1, typeof(Data?));
            Assert.IsNotNull(result);
            Assert.IsTrue(((Data?)result).Value == Data.Var1);
        }

        [TestMethod]
        public virtual void Test_Can_Convert_Guid_To_String()
        {
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(Guid.NewGuid(), typeof(Guid));
            Assert.IsNotNull(result);
            Asserts.IsType(typeof(string), result);
            string bResult = (string)result;
            Assert.IsTrue(!string.IsNullOrEmpty(bResult));
        }

        [TestMethod]
        public virtual void Test_Can_Parse_DateTimeOffset()
        {
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(DateTimeOffset.Now.ToString(), typeof(DateTimeOffset));
            Assert.IsNotNull(result);
            Asserts.IsType(typeof(DateTimeOffset), result);
        }

        [TestMethod]
        public virtual void Test_Can_Convert_DateTime()
        {
            StandardCLRConverter tConverter = new StandardCLRConverter();
            object result = tConverter.ConvertToType(DateTime.Now.ToString(), typeof(DateTime));
            Assert.IsNotNull(result);
            Asserts.IsType(typeof(DateTime), result);
        }

        [TestMethod]
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
                Assert.IsNotNull(result);
                Asserts.IsType(typeof(int), result);
            }
        }

        [TestMethod]
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
                Assert.IsNotNull(result);
                Asserts.IsType(typeof(double), result);
            }
        }

        [TestMethod]
        public virtual void Test_Can_Convert_Using_Template()
        {
            string item = "123.12";

            StandardCLRConverter tConverter = new StandardCLRConverter();
            double result = tConverter.ConvertToType<double>(item);
            Assert.IsTrue(result == 123.12);
        }
    }
}
