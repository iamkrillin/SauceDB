using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using DataAccess.PostgreSQL;
using DataAccess.SqlCompact;
using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests
{
    public class MySQLTypeConverterTests
    {
        [Fact]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeChar });
            Assert.Equal("VARCHAR(1) CHARSET utf8", type);
        }

        [Fact]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeText });
            Assert.Equal("LONGTEXT CHARSET utf8", type);
        }

        [Fact]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.Text });
            Assert.Equal("LONGTEXT", type);
        }

        [Fact]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeString });
            Assert.Equal("VARCHAR(200) CHARSET utf8", type);
        }

        [Fact]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR(200)", type);
        }

        [Fact]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = 400 });
            Assert.Equal("VARCHAR(400)", type);
        }

        [Fact]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = Int32.MaxValue });
            Assert.Equal("LONGTEXT", type);
        }

        [Fact]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(int), new Core.Data.DataFieldInfo() { });
            Assert.Equal("INT(11)", type);
        }

        [Fact]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(long), new Core.Data.DataFieldInfo() { });
            Assert.Equal("BIGINT(20)", type);
        }

        [Fact]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(double), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DOUBLE", type);

            type = mapper.MapType(typeof(float), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DOUBLE", type);

            type = mapper.MapType(typeof(decimal), new Core.Data.DataFieldInfo() { });
            Assert.Equal("NUMERIC", type);
        }

        [Fact]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(byte[]), new Core.Data.DataFieldInfo() { });
            Assert.Equal("LONGBLOB", type);
        }

        [Fact]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(bool), new Core.Data.DataFieldInfo() { });
            Assert.Equal("TINYINT(1)", type);
        }

        [Fact]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(DateTime), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DATETIME", type);
        }

        [Fact]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new Core.Data.DataFieldInfo() { });
            Assert.Equal("TIME", type);
        }

        [Fact]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR(1)", type);
        }

        [Fact]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new MySqlServerDBConverter();

            string type = mapper.MapType(this.GetType(), new Core.Data.DataFieldInfo() { });
            Assert.Equal("VARCHAR(200)", type);
        }
    }
}
