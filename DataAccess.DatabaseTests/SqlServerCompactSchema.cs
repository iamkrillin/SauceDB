using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.SqlCompact;
namespace DataAccess.DatabaseTests
{
    public class SqlServerCompactSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            IDataStore toReturn = SqlCompactConnection.GetDataStore("C:\\Data.sdf");
            IDbCommand cmd = toReturn.Connection.GetCommand();
            return toReturn;
        }

        [Fact]
        public override void Test_Honors_Field_Length()
        {
            base.Test_Honors_Field_Length();
        }

        [Fact]
        public override void Test_Can_Modify_Column_And_Keep_Default_Value()
        {
        }
    }
}
