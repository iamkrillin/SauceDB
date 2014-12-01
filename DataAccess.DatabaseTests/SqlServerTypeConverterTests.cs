using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests
{
    public class SqlServerTypeConverterTests
    {
        [Fact]
        public void Test_Can_Convert_Unicode_Char()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeChar });
            Assert.Equal("nvarchar(1)", type);
        }

        [Fact]
        public void Test_Can_Convert_Unicode_Text()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeText });
            Assert.Equal("NTEXT", type);
        }

        [Fact]
        public void Test_Can_Convert_Text()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.Text });
            Assert.Equal("TEXT", type);
        }

        [Fact]
        public void Test_Can_Convert_Unicode_String()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();
            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { DataFieldType = Core.Attributes.FieldType.UnicodeString });
            Assert.Equal("nvarchar(200)", type);
        }

        [Fact]
        public void Test_Can_Convert_String()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { });
            Assert.Equal("varchar(200)", type);
        }

        [Fact]
        public void Test_Can_Override_String_Length()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = 400 });
            Assert.Equal("varchar(400)", type);
        }

        [Fact]
        public void Test_Can_Override_String_Length_With_Max()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(string), new Core.Data.DataFieldInfo() { FieldLength = Int32.MaxValue });
            Assert.Equal("varchar(MAX)", type);
        }

        [Fact]
        public void Test_Can_Convert_Int32()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(int), new Core.Data.DataFieldInfo() { });
            Assert.Equal("int", type);
        }

        [Fact]
        public void Test_Can_Convert_Int64()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(long), new Core.Data.DataFieldInfo() { });
            Assert.Equal("bigint", type);
        }

        [Fact]
        public void Test_Can_Convert_Float()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(double), new Core.Data.DataFieldInfo() { });
            Assert.Equal("real", type);

            type = mapper.MapType(typeof(float), new Core.Data.DataFieldInfo() { });
            Assert.Equal("real", type);

            type = mapper.MapType(typeof(decimal), new Core.Data.DataFieldInfo() { });
            Assert.Equal("Money", type);
        }

        [Fact]
        public void Test_Can_Convert_ByteArray()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(byte[]), new Core.Data.DataFieldInfo() { });
            Assert.Equal("varbinary(MAX)", type);
        }

        [Fact]
        public void Test_Can_Convert_Bool()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(bool), new Core.Data.DataFieldInfo() { });
            Assert.Equal("bit", type);
        }

        [Fact]
        public void Test_Can_Convert_dates()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(DateTime), new Core.Data.DataFieldInfo() { });
            Assert.Equal("DATETIME", type);
        }

        [Fact]
        public void Test_Can_Convert_timespan()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(TimeSpan), new Core.Data.DataFieldInfo() { });
            Assert.Equal("time(7)", type);
        }

        [Fact]
        public void Test_Can_Convert_char()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(typeof(char), new Core.Data.DataFieldInfo() { });
            Assert.Equal("varchar(1)", type);
        }

        [Fact]
        public void Test_Can_Get_Default()
        {
            IConvertToDatastore mapper = new SqlServerDBConverter();

            string type = mapper.MapType(this.GetType(), new Core.Data.DataFieldInfo() { });
            Assert.Equal("varchar(200)", type);
        }
    }
}
