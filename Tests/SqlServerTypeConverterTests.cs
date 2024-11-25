using DataAccess.Core.Attributes;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    [TestClass]
    public class SqlServerTypeConverterTests
    {
        [TestMethod]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(char), new DataFieldInfo() { DataFieldType = FieldType.UnicodeChar });
            Assert.AreEqual("nvarchar(1)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.UnicodeText });
            Assert.AreEqual("NTEXT", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.Text });
            Assert.AreEqual("TEXT", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.UnicodeString });
            Assert.AreEqual("nvarchar(200)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { });
            Assert.AreEqual("varchar(200)", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { FieldLength = 400 });
            Assert.AreEqual("varchar(400)", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { FieldLength = int.MaxValue });
            Assert.AreEqual("varchar(MAX)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(int), new DataFieldInfo() { });
            Assert.AreEqual("int", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(long), new DataFieldInfo() { });
            Assert.AreEqual("bigint", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(double), new DataFieldInfo() { });
            Assert.AreEqual("real", type);

            type = mapper.MapType(typeof(float), new DataFieldInfo() { });
            Assert.AreEqual("real", type);

            type = mapper.MapType(typeof(decimal), new DataFieldInfo() { });
            Assert.AreEqual("Money", type);
        }

        [TestMethod]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(byte[]), new DataFieldInfo() { });
            Assert.AreEqual("varbinary(MAX)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(bool), new DataFieldInfo() { });
            Assert.AreEqual("bit", type);
        }

        [TestMethod]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(DateTime), new DataFieldInfo() { });
            Assert.AreEqual("DATETIME", type);
        }

        [TestMethod]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new DataFieldInfo() { });
            Assert.AreEqual("time(7)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(char), new DataFieldInfo() { });
            Assert.AreEqual("varchar(1)", type);
        }

        [TestMethod]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(GetType(), new DataFieldInfo() { });
            Assert.AreEqual("varchar(200)", type);
        }
    }
}
