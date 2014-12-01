using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using DataAccess.Core.Data;
using System.Data.SQLite;
using DataAccess.Core;
using DataAccess.DatabaseTests.DataObjects;
using System.IO;

namespace DataAccess.DatabaseTests
{
    /// <summary>
    /// Summary description for SqLiteSchema
    /// </summary>
    public class SqLiteSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }

        [Fact]
        public override void Test_Can_RemoveColumn()
        {
            Assert.Throws(typeof(DataStoreException), () =>
                {
                    base.Test_Can_RemoveColumn();
                });
        }

        [Fact]
        public override void Test_Honors_Field_Length()
        {//sqllite doesnt do this near as I can tell
        }

        [Fact]
        public override void Test_Can_Get_Tables()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.True(tables.Count() > 0);
            foreach (DBObject t in tables)
            {
                if (t.Name.StartsWith("sqlite")) continue;
                Assert.True(t != null);
                Assert.True(t.Name != "");
                Assert.True(t.Schema != "");
                Assert.NotNull(t.Columns);
                Assert.True(t.Columns.Count > 0);

                foreach (Column c in t.Columns)
                {
                    Assert.True(c != null);
                    Assert.True(c.Name != "");
                }
            }
        }

        [Fact]
        public override void Test_DB_Types_Come_Back_Right()
        {
            dstore.LoadEntireTable<DBTypeTestObject>();
            IEnumerable<DBObject> objects = dstore.SchemaValidator.TableValidator.GetObjects(true);
            Assert.True(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("DBTypeTestObjects", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore = SqlLiteDataConnection.GetDataStore(@"C:\TestFile.sqlite");
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.True(false);
            };

            dstore.LoadEntireTable<DBTypeTestObject>();
        }

        public override void Test_Can_Modify_Column_And_Keep_Default_Value()
        {
        }

        public override void Test_Can_Modify_Column_Type()
        {
        }
    }
}
