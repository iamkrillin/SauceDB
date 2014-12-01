using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using DataAccess.Core.Data;
using System.Data;
using DataAccess.DatabaseTests.DataObjects;

namespace DataAccess.DatabaseTests
{
    public class PostgreSchema : SchemaValidatorTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore("Server=postgre;Port=5432;Database=Sauce;User Id=sauce;Password=Sauce;");
        }

        public PostgreSchema()
        {
            dstore = GetDataStore();
            InitHelper.InitDataStore(dstore);
        }

        [Fact]
        public override void Test_Can_Get_Tables()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.True(tables.Count() > 0);
            foreach (DBObject t in tables)
            {
                Assert.True(t != null);
                Assert.True(t.Name != "");
                Assert.NotNull(t.Columns);
                Assert.True(t.Columns.Count > 0);

                foreach (Column c in t.Columns)
                {
                    Assert.True(c != null);
                    Assert.True(c.Name != "");
                }
            }
        }

        public override void Test_Can_Modify_Column_Type()
        {
            TypeInfo ti1 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.True(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.ID = Guid.NewGuid().ToString();
            tipk.Date = "2010-11-20";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);


            TypeInfo ti2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.True(ti2 != null);
        }
    }
}
