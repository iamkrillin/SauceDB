using DataAccess.Core.Attributes;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using DataAccess.PostgreSQL;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    [TestClass]
    public class MySQLTypeConverterTests
    {
        [TestMethod]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(char), new DataFieldInfo() { DataFieldType = FieldType.UnicodeChar });
            Assert.AreEqual("VARCHAR(1) CHARSET utf8", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.UnicodeText });
            Assert.AreEqual("LONGTEXT CHARSET utf8", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.Text });
            Assert.AreEqual("LONGTEXT", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.UnicodeString });
            Assert.AreEqual("VARCHAR(200) CHARSET utf8", type);
        }

        [TestMethod]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR(200)", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { FieldLength = 400 });
            Assert.AreEqual("VARCHAR(400)", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { FieldLength = int.MaxValue });
            Assert.AreEqual("LONGTEXT", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(int), new DataFieldInfo() { });
            Assert.AreEqual("INT(11)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(long), new DataFieldInfo() { });
            Assert.AreEqual("BIGINT(20)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(double), new DataFieldInfo() { });
            Assert.AreEqual("DOUBLE", type);

            type = mapper.MapType(typeof(float), new DataFieldInfo() { });
            Assert.AreEqual("DOUBLE", type);

            type = mapper.MapType(typeof(decimal), new DataFieldInfo() { });
            Assert.AreEqual("NUMERIC", type);
        }

        [TestMethod]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(byte[]), new DataFieldInfo() { });
            Assert.AreEqual("LONGBLOB", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(bool), new DataFieldInfo() { });
            Assert.AreEqual("TINYINT(1)", type);
        }

        [TestMethod]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(DateTime), new DataFieldInfo() { });
            Assert.AreEqual("DATETIME", type);
        }

        [TestMethod]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new DataFieldInfo() { });
            Assert.AreEqual("TIME", type);
        }

        [TestMethod]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(char), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR(1)", type);
        }

        [TestMethod]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(GetType(), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR(200)", type);
        }
    }
}
