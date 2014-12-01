using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.Core.Interfaces;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core;
using DataAccess.Core.Data;

namespace DataAccess.DatabaseTests.Tests
{
    public abstract class FunctionTests : IDisposable
    {
        protected IDataStore dstore;
        public abstract IDataStore GetDataStore();

        public void Dispose()
        {
            Init();
        }

        public FunctionTests()
        {
            Init();
        }

        private void Init()
        {
            dstore = GetDataStore();
            dstore.InitDataStore();
        }

        [Fact]
        public virtual void Test_Can_Insert_Many_Items()
        {
            int numItems = 20;
            List<TestBulkItem> items = new List<TestBulkItem>();
            for (int i = 0; i < numItems; i++)
            {
                items.Add(new TestBulkItem()
                {
                    one = "one",
                    two = "two",
                    three = "three",
                    four = "four"
                });
            }

            dstore.InsertObjects(items);

            List<TestBulkItem> loaded = dstore.LoadEntireTable<TestBulkItem>().ToList();
            Assert.True(loaded.Count == numItems);
        }

        [Fact]
        public virtual void Test_Can_Load_Object_With_Enums()
        {
            Assert.True(dstore.InsertObject(new TestItemWithEnum()
                {
                    ValueOne = Data.Var1,
                    ValueTwo = Data.Var2,
                    AnotherValue = 12
                }));

            TestItemWithEnum loaded = dstore.LoadObject<TestItemWithEnum>(1);
            Assert.NotNull(loaded);
            Assert.True(loaded.ValueOne == Data.Var1);
            Assert.True(loaded.ValueTwo.Value == Data.Var2);
            Assert.True(loaded.ValueTwo.Value == Data.Var2);
            Assert.True(loaded.AnotherValue.Value == 12);
            Assert.False(loaded.AnotherValueTwo.HasValue);
        }

#if(!DEBUG)
        [Fact]
        public virtual void Test_Bulk_Insert()
        {
            int numItems = 300;
            List<TestBulkItem> items = new List<TestBulkItem>();
            for (int i = 0; i < numItems; i++)
            {
                items.Add(new TestBulkItem()
                {
                    one = "one",
                    two = "two",
                    three = "three",
                    four = "four"
                });
            }

            dstore.InsertObjects(items);

            List<TestBulkItem> loaded = dstore.LoadEntireTable<TestBulkItem>().ToList();
            Assert.True(loaded.Count == numItems);
        }
#endif
        [Fact]
        public virtual void Test_Can_Get_Data_Connection()
        {
            Assert.NotNull(dstore.Connection.GetConnection());
        }

        [Fact]
        public virtual void Test_Can_Get_Data_Command()
        {
            Assert.NotNull(dstore.Connection.GetCommand());
        }

        [Fact]
        public virtual void Test_Can_Get_Type_Converter()
        {
            Assert.NotNull(dstore.Connection.CLRConverter);
        }

        [Fact]
        public abstract void Test_Can_Get_Escape_Sequences();

        [Fact]
        public virtual void Test_Can_Load_Object()
        {
            Test_Can_Insert_Object();
            TestItem ti = new TestItem();
            ti.id = 1;

            Assert.True(dstore.LoadObject(ti));
            Assert.True(!string.IsNullOrEmpty(ti.Something));
            Assert.True(ti.Something.Equals("SomethingNew", StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public virtual void Test_Can_Load_Object_By_Key()
        {
            TestItem newObject = new TestItem();
            newObject.Something = "A Test String";
            Assert.True(dstore.InsertObject(newObject));
            TestItem ti = dstore.LoadObject(typeof(TestItem), newObject.id) as TestItem;

            Assert.True(dstore.LoadObject(ti));
            Assert.True(!string.IsNullOrEmpty(ti.Something));
            Assert.True(ti.id == 1);
            Assert.True(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public virtual void Test_Can_Load_Object_By_Key_Templeted()
        {
            TestItem newObject = new TestItem();
            newObject.Something = "A Test String";
            Assert.True(dstore.InsertObject(newObject));

            TestItem ti = dstore.LoadObject<TestItem>(newObject.id);

            Assert.True(dstore.LoadObject(ti));
            Assert.True(!string.IsNullOrEmpty(ti.Something));
            Assert.True(ti.id == 1);
            Assert.True(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));

            ti = dstore.LoadObject<TestItem>(1, false);

            Assert.True(dstore.LoadObject(ti));
            Assert.True(!string.IsNullOrEmpty(ti.Something));
            Assert.True(ti.id == 1);
            Assert.True(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));
        }

        [Fact]
        public virtual void Test_Cannot_Load_By_Key_With_Multiple_Keys()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                TestItem ti = dstore.LoadObject(typeof(TestItemTwoKeys), 1) as TestItem;
            });
        }

        [Fact]
        public virtual void Test_Can_Insert_Object()
        {
            TestItem ti = new TestItem();
            ti.Something = "SomethingNew";
            Assert.True(dstore.InsertObject(ti));
        }

        [Fact]
        public virtual void Test_Get_Key_For_Item()
        {
            TestItem ti = new TestItem();
            ti.id = 1;

            Assert.True(((int)dstore.GetKeyForItemType(typeof(TestItem), ti)) == 1);
        }

        [Fact]
        public virtual void Test_Get_Key_Fails_For_Many_Keys()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                dstore.GetKeyForItemType(typeof(TestItemTwoKeys), null);
            });
        }

        [Fact]
        public virtual void Test_Get_Key_Fails_For_No_Keys()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                dstore.GetKeyForItemType(typeof(TestItemNoKey), null);
            });
        }

        [Fact]
        public virtual void Test_Can_Delete_Item()
        {
            Test_Can_Insert_Object();
            List<TestItem> items = dstore.LoadEntireTable<TestItem>().OrderBy(R => R.id).ToList();
            Assert.True(dstore.DeleteObject((items.Last())));
            Assert.Null(dstore.LoadObject<TestItem>(items.Last().id));
        }

        [Fact]
        public virtual void Test_Can_Delete_Item_By_Key()
        {
            Test_Can_Insert_Object();
            List<TestItem> items = dstore.LoadEntireTable<TestItem>().OrderBy(R => R.id).ToList();
            Assert.True(dstore.DeleteObject(typeof(TestItem), items.Last().id));
            Assert.Null(dstore.LoadObject<TestItem>(items.Last().id));
        }

        [Fact]
        public virtual void Test_Can_Delete_Item_Templeted()
        {
            Test_Can_Insert_Object();
            List<TestItem> items = dstore.LoadEntireTable<TestItem>().OrderBy(R => R.id).ToList();
            Assert.True(dstore.DeleteObject<TestItem>((items.Last().id)));
            Assert.Null(dstore.LoadObject<TestItem>(items.Last().id));
        }

        [Fact]
        public virtual void Test_Can_Load_Entire_Table()
        {
            IEnumerable<object> items = dstore.LoadEntireTable(typeof(TestItem));
            Assert.True(items != null);
            foreach (object o in items)
            {
                Assert.True(o != null);
            }
        }

        [Fact]
        public virtual void Test_Can_Load_Entire_Table_Templeted()
        {
            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>();
            Assert.True(items != null);
            foreach (TestItem o in items)
            {
                Assert.True(o != null);
                Assert.True(o.id != 0);
                Assert.True(!string.IsNullOrEmpty(o.Something));
            }
        }

        [Fact]
        public virtual void Test_Is_new()
        {
            Test_Can_Insert_Object();
            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>();
            Assert.True(!dstore.IsNew(items.ElementAt(0)));
        }

        [Fact]
        public virtual void Test_Can_Insert_Multiple_Items()
        {
            List<TestItem> items = new List<TestItem>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(new TestItem() { Something = Guid.NewGuid().ToString() });
            }
            Assert.True(dstore.InsertObjects(items));
        }

        [Fact]
        public virtual void Test_Can_Save_Object()
        {
            TestItemAdditionalInit newItem = new TestItemAdditionalInit();
            newItem.Something = "a";
            dstore.SaveObject(newItem);

            Assert.True(newItem.id > 0);
            Assert.NotNull(dstore.LoadObject<TestItemAdditionalInit>(1));
        }

        [Fact]
        public virtual void Test_Additional_Init_Is_Called()
        {
            TestItemAdditionalInit newItem = new TestItemAdditionalInit();
            newItem.Something = "a";
            dstore.InsertObject(newItem);

            Assert.True(dstore.LoadObject(newItem));
            Assert.True(!string.IsNullOrEmpty(newItem.Something));
            Assert.True(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.True(newItem.Calculated == 15);
        }

        [Fact]
        public virtual void Test_Additional_Init_With_DataStore_Parm_Is_Called()
        {
            TestItemAdditionalInitWithParm newItem = new TestItemAdditionalInitWithParm();
            newItem.Something = "a";
            dstore.InsertObject(newItem);

            Assert.True(dstore.LoadObject(newItem));
            Assert.True(!string.IsNullOrEmpty(newItem.Something));
            Assert.True(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.True(newItem.Calculated > 0);
        }

        [Fact]
        public virtual void Test_Additional_Init_With_Bad_Parm_Fails()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                TestItemAdditionalInitWithBadParm newItem = new TestItemAdditionalInitWithBadParm();
                newItem.Something = "a";
                dstore.InsertObject(newItem);

                Assert.True(dstore.LoadObject(newItem));
                Assert.True(!string.IsNullOrEmpty(newItem.Something));
                Assert.True(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
                Assert.True(newItem.Calculated > 0);
            });
        }

        [Fact]
        public virtual void Test_Can_Update_Item()
        {
            Test_Can_Insert_Object();
            IList<TestItem> items = dstore.LoadEntireTable<TestItem>().OrderBy(R => R.id).ToList();
            Assert.True(items.Count > 0);
            TestItem ti = items.Last();

            string value = ti.Something;
            ti.Something = Guid.NewGuid().ToString();

            Assert.True(dstore.UpdateObject(ti));

            dstore.LoadObject(ti);
            Assert.True(ti.Something != value);
        }

        [Fact]
        public virtual void QueryDataConstructorTest()
        {
            using (QueryData target = new QueryData())
            {
                Assert.False(target.QuerySuccessful);
            }
        }

        [Fact]
        public virtual void Test_Can_Parse_Object()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItem));
            Assert.NotNull(ti);

            Assert.NotNull(ti.DataFields);
            foreach (DataFieldInfo dfi in ti.DataFields)
            {
                Assert.True(dfi.LoadField);
                if (dfi.PrimaryKey) Assert.False(dfi.SetOnInsert);
                else Assert.True(dfi.SetOnInsert);
                Assert.True(!string.IsNullOrEmpty(dfi.FieldName));
                Assert.NotNull(dfi.Getter);
                Assert.NotNull(dfi.Setter);
            }


            Assert.False(ti.BypassValidation);
            Assert.NotNull(ti.AdditionalInit);
            Assert.True(ti.AdditionalInit.Count == 0);
        }

        [Fact]
        public virtual void Test_Can_parse_AdditionalInit()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemAdditionalInit));
            Assert.NotNull(ti);
            Assert.NotNull(ti.AdditionalInit);
            Assert.True(ti.AdditionalInit.Count == 1);
        }

        [Fact]
        public virtual void Test_Can_parse_ByPass()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemBypassValidation));
            Assert.NotNull(ti);
            Assert.True(ti.BypassValidation);
        }

        [Fact]
        public virtual void Test_Can_Parse_Foreign_Key()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemWithForeignKey));
            Assert.NotNull(ti);
            Assert.NotNull(ti.DataFields);
            var target = ti.DataFields.Where(R => R.PrimaryKeyType != null).FirstOrDefault();
            Assert.NotNull(target);
        }

        [Fact]
        public virtual void Test_Can_Parse_Key_Attribute()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemWithKeyAttribute));
            Assert.NotNull(ti);
            Assert.NotNull(ti.DataFields);
            var target = ti.DataFields.Where(R => R.PrimaryKey).FirstOrDefault();
            Assert.NotNull(target);
        }

        [Fact]
        public virtual void Test_Can_Parse_Table_Attribute()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemNewTableName));
            Assert.NotNull(ti);
            Assert.True(ti.TableName.Contains("SomeNewTable"));

            ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemNewSchema));
            Assert.NotNull(ti);
            Assert.True(ti.Schema.Contains("NewSchema"));
        }

        [Fact]
        public virtual void Test_Can_get_primary_Keys()
        {
            var nonPrimary = dstore.TypeInformationParser.GetPrimaryKeys(typeof(TestItemWithKeyAttribute)).Where(R => !R.PrimaryKey).FirstOrDefault();
            Assert.Null(nonPrimary);
        }

        [Fact]
        public virtual void Test_Can_Get_Type_Fields()
        {
            IEnumerable<DataFieldInfo> items = dstore.TypeInformationParser.GetTypeFields(typeof(TestItem));
            Assert.NotNull(items);
            Assert.NotNull(items.Count() > 0);
        }

        [Fact]
        public virtual void Test_Ignored_Field_Is_Ignored()
        {
            IEnumerable<DataFieldInfo> fields = dstore.TypeInformationParser.GetTypeFields(typeof(TestItemIgnoredField));
            Assert.True(fields != null);
            Assert.True(fields.Count() == 2);
            var ignored = fields.Where(R => R.PropertyName.Equals("Ignored", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(ignored == null);
        }

        [Fact]
        public virtual void Test_Can_Parse_View_Attribute()
        {
            try
            {
                dstore.TypeInformationParser.GetTypeFields(typeof(ViewObject));
            }
            catch
            {//view is not there so will throw exception but thats cool, should have parsed it

            }

            IEnumerable<DataFieldInfo> fields = dstore.TypeInformationParser.GetTypeFields(typeof(ViewObject));
            Assert.True(fields != null);
            Assert.True(fields.Count() == 1);
        }

        [Fact]
        public virtual void CanStartTransaction()
        {
            dstore.StartTransaction();
        }

        [Fact]
        public virtual void CanCommitTransaction()
        {
            dstore.TypeInformationParser.GetTypeInfo(typeof(TestItem));
            TestItem ti;
            using (var context = dstore.StartTransaction())
            {
                ti = new TestItem()
                    {
                        Something = "foo"
                    };

                context.Instance.InsertObject(ti);
                context.Commit();
            }

            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>();
            Assert.True(items.Count() == 1);
        }

        [Fact]
        public virtual void CanRollbackTransaction()
        {
            dstore.TypeInformationParser.GetTypeInfo(typeof(TestItem));
            TestItem ti = new TestItem()
            {
                Something = "foo"
            };

            dstore.InsertObject(ti);
            Assert.True(dstore.LoadObject<TestItem>(ti.id).Something == ti.Something);

            using (var context = dstore.StartTransaction())
            {
                ti.Something = "bar";
                context.Instance.UpdateObject(ti);
                context.Rollback();
            }

            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>().ToList();
            Assert.True(items.Count() == 1);
            Assert.True(items.First().Something == "foo");
        }

        [Fact]
        public virtual void Can_Do_IN()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "foo" });

            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>().ToList();
            List<int> ids = items.Select(r => r.id).ToList();
            IEnumerable<TestItem> items2 = dstore.LoadObjects<TestItem>(ids).ToList();

            Assert.True(items.Count() == items2.Count());
        }

        [Fact]
        public virtual void Can_Do_Command_With_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems where Something = @query", new { query = "foo" });
            Assert.True(items != null);
            Assert.True(items.Count() == 1);
        }

        [Fact]
        public virtual void Can_Do_Command_Without_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems");
            Assert.True(items != null);
            Assert.True(items.Count() == 3);
        }

        [Fact]
        public virtual void Can_Do_Command_And_Set_Markers()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });
            var command = dstore.GetCommand<TestItem>();
            command.SetCommandText("select {{selectlist}} from {{tablename}} where Something = @query", "{{selectlist}}", "{{tablename}}");
            command.SetParameters(new { query = "foo" });

            var items = command.ExecuteCommandGetList();
            Assert.True(items != null);
            Assert.True(items.Count() == 1);
        }

        [Fact]
        public virtual void Private_Additional_Init_Is_Called()
        {
            dstore.InsertObject(new TestItemPrivateInitMethod() { Name = "foo" });
            Assert.True(dstore.LoadObject<TestItemPrivateInitMethod>(1).Length == 3);
        }

        [Fact]
        public virtual void Additional_Init_Is_Called_When_On_Parent_Class()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            Assert.True(dstore.LoadObject<ChildClassWIithParentPrivateInitMethod>(1).Length == 3);
        }

        [Fact]
        public void Test_Update_Is_Closing_Connections()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                dstore.UpdateObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString(), ID = 1 });
            }
        }

        [Fact]
        public void Test_Insert_Is_Closing_Connections()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString() });
            }
        }

        [Fact]
        public void Test_Transaction_Is_Closing_Connections()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                using (TransactionContext ctx = dstore.StartTransaction())
                {
                    ctx.Instance.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString() });
                    ctx.Commit();
                }
            }
        }
    }
}
