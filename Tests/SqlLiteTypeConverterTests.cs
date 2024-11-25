using DataAccess.Core.Attributes;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    [TestClass]
    public class SqlLiteTypeConverterTests
    {
        [TestMethod]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(char), new DataFieldInfo() { DataFieldType = FieldType.UnicodeChar });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.UnicodeText });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.Text });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(string), new DataFieldInfo() { DataFieldType = FieldType.UnicodeString });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { FieldLength = 400 });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(string), new DataFieldInfo() { FieldLength = int.MaxValue });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(int), new DataFieldInfo() { });
            Assert.AreEqual("INTEGER", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(long), new DataFieldInfo() { });
            Assert.AreEqual("INTEGER", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(double), new DataFieldInfo() { });
            Assert.AreEqual("DOUBLE", type);

            type = mapper.MapType(typeof(float), new DataFieldInfo() { });
            Assert.AreEqual("DOUBLE", type);

            type = mapper.MapType(typeof(decimal), new DataFieldInfo() { });
            Assert.AreEqual("DECIMAL", type);
        }

        [TestMethod]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(byte[]), new DataFieldInfo() { });
            Assert.AreEqual("BLOB", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(bool), new DataFieldInfo() { });
            Assert.AreEqual("BOOL", type);
        }

        [TestMethod]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(DateTime), new DataFieldInfo() { });
            Assert.AreEqual("DATETIME", type);
        }

        [TestMethod]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(char), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR", type);
        }

        [TestMethod]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(GetType(), new DataFieldInfo() { });
            Assert.AreEqual("VARCHAR", type);
        }
    }
}
