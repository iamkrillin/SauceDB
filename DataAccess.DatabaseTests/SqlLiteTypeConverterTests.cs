using DataAccess.Core.Interfaces;
using DataAccess.SqlCompact;
using DataAccess.SQLite;
using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests
{
    public class SqlLiteTypeConverterTests
    {
        [Fact]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeChar });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeText });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.Text });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeString });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = 400 });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = Int32.MaxValue });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(int), new Core.Data.DataFieldInfo() { });
            Assert.Equal("INTEGER", type);
        }

        [Fact]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(long), new Core.Data.DataFieldInfo() { });
            Assert.Equal("INTEGER", type);
        }

        [Fact]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(double), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DOUBLE", type);

            type = mapper.MapType(typeof(float), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DOUBLE", type);

            type = mapper.MapType(typeof(decimal), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DECIMAL", type);
        }

        [Fact]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(byte[]), new Core.Data.DataFieldInfo() { });
            Assert.Equal("BLOB", type);
        }

        [Fact]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(bool), new Core.Data.DataFieldInfo() { });
            Assert.Equal("BOOL", type);
        }

        [Fact]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(DateTime), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DATETIME", type);
        }

        [Fact]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR", type);
        }

        [Fact]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new SQLiteDBConverter();

            string type = mapper.MapType(this.GetType(), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR", type);
        }
    }
}
