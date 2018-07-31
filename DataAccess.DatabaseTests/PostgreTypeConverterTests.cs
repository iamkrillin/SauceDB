using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class PostgreTypeConverterTests
    {
        [TestMethod]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();
            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeChar });
            Assert.AreEqual("character", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeText });
            Assert.AreEqual("text", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.Text });
            Assert.AreEqual("text", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeString });
            Assert.AreEqual("character varying", type);
        }

        [TestMethod]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("character varying", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = 400 });
            Assert.AreEqual("character varying", type);
        }

        [TestMethod]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = Int32.MaxValue });
            Assert.AreEqual("character varying", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(int), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("INTEGER", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(long), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("bigint", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(double), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("real", type);

            type = mapper.MapType(typeof(float), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("real", type);

            type = mapper.MapType(typeof(decimal), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("NUMERIC", type);
        }

        [TestMethod]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(byte[]), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("bytea[]", type);
        }

        [TestMethod]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(bool), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("boolean", type);
        }

        [TestMethod]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(DateTime), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("date", type);
        }

        [TestMethod]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("INTERVAL", type);
        }

        [TestMethod]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("character", type);
        }

        [TestMethod]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new PostgreDBConverter();

            string type = mapper.MapType(this.GetType(), new Core.Data.DataFieldInfo() { });
            Assert.AreEqual("character varying", type);
        }
    }
}
