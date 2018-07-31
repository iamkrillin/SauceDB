using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core;
using DataAccess.Core.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests.Tests
{
    [TestClass]
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

        [TestMethod]
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
            Assert.IsTrue(loaded.Count == numItems);
        }

        [TestMethod]
        public virtual void Test_Can_Load_Object_With_Enums()
        {
            Assert.IsTrue(dstore.InsertObject(new TestItemWithEnum()
            {
                ValueOne = Data.Var1,
                ValueTwo = Data.Var2,
                AnotherValue = 12
            }));

            TestItemWithEnum loaded = dstore.LoadObject<TestItemWithEnum>(1);
            Assert.IsNotNull(loaded);
            Assert.IsTrue(loaded.ValueOne == Data.Var1);
            Assert.IsTrue(loaded.ValueTwo.Value == Data.Var2);
            Assert.IsTrue(loaded.ValueTwo.Value == Data.Var2);
            Assert.IsTrue(loaded.AnotherValue.Value == 12);
            Assert.IsFalse(loaded.AnotherValueTwo.HasValue);
        }

#if(!DEBUG)
        [TestMethod]
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
            Assert.IsTrue(loaded.Count == numItems);
        }
#endif
        [TestMethod]
        public virtual void Test_Can_Get_Data_Connection()
        {
            Assert.IsNotNull(dstore.Connection.GetConnection());
        }

        [TestMethod]
        public virtual void Test_Can_Get_Data_Command()
        {
            Assert.IsNotNull(dstore.Connection.GetCommand());
        }

        [TestMethod]
        public virtual void Test_Can_Get_Type_Converter()
        {
            Assert.IsNotNull(dstore.Connection.CLRConverter);
        }

        [TestMethod]
        public abstract void Test_Can_Get_Escape_Sequences();

        [TestMethod]
        public virtual void Test_Can_Load_Object()
        {
            Test_Can_Insert_Object();
            TestItem ti = new TestItem();
            ti.id = 1;

            Assert.IsTrue(dstore.LoadObject(ti));
            Assert.IsTrue(!string.IsNullOrEmpty(ti.Something));
            Assert.IsTrue(ti.Something.Equals("SomethingNew", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public virtual void Test_Can_Load_Object_By_Key_Templeted()
        {
            TestItem newObject = new TestItem();
            newObject.Something = "A Test String";
            Assert.IsTrue(dstore.InsertObject(newObject));

            TestItem ti = dstore.LoadObject<TestItem>(newObject.id);

            Assert.IsTrue(dstore.LoadObject(ti));
            Assert.IsTrue(!string.IsNullOrEmpty(ti.Something));
            Assert.IsTrue(ti.id == 1);
            Assert.IsTrue(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));

            ti = dstore.LoadObject<TestItem>(1);

            Assert.IsTrue(dstore.LoadObject(ti));
            Assert.IsTrue(!string.IsNullOrEmpty(ti.Something));
            Assert.IsTrue(ti.id == 1);
            Assert.IsTrue(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public virtual void Test_Can_Insert_Object()
        {
            TestItem ti = new TestItem();
            ti.Something = "SomethingNew";
            Assert.IsTrue(dstore.InsertObject(ti));
        }

        [TestMethod]
        public virtual void Test_Get_Key_For_Item()
        {
            TestItem ti = new TestItem();
            ti.id = 1;

            Assert.IsTrue(((int)dstore.GetKeyForItemType(typeof(TestItem), ti)) == 1);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual void Test_Get_Key_Fails_For_Many_Keys()
        {
            dstore.GetKeyForItemType(typeof(TestItemTwoKeys), null);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual void Test_Get_Key_Fails_For_No_Keys()
        {
            dstore.GetKeyForItemType(typeof(TestItemNoKey), null);
        }

        [TestMethod]
        public virtual void Test_Can_Delete_Item()
        {
            Test_Can_Insert_Object();
            List<TestItem> items = dstore.LoadEntireTable<TestItem>().OrderBy(R => R.id).ToList();
            Assert.IsTrue(dstore.DeleteObject(items.Last()));
            Assert.IsNull(dstore.LoadObject<TestItem>(items.Last().id));
        }

        [TestMethod]
        public virtual void Test_Can_Load_Entire_Table()
        {
            IEnumerable<object> items = dstore.LoadEntireTable(typeof(TestItem));
            Assert.IsTrue(items != null);
            foreach (object o in items)
            {
                Assert.IsTrue(o != null);
            }
        }

        [TestMethod]
        public virtual void Test_Can_Load_Entire_Table_Templeted()
        {
            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>();
            Assert.IsTrue(items != null);
            foreach (TestItem o in items)
            {
                Assert.IsTrue(o != null);
                Assert.IsTrue(o.id != 0);
                Assert.IsTrue(!string.IsNullOrEmpty(o.Something));
            }
        }

        [TestMethod]
        public virtual void Test_Is_new()
        {
            Test_Can_Insert_Object();
            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>();
            Assert.IsTrue(!dstore.IsNew(items.ElementAt(0)));
        }

        [TestMethod]
        public virtual void Test_Can_Insert_Multiple_Items()
        {
            List<TestItem> items = new List<TestItem>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(new TestItem() { Something = Guid.NewGuid().ToString() });
            }
            Assert.IsTrue(dstore.InsertObjects(items));
        }

        [TestMethod]
        public virtual void Test_Can_Save_Object()
        {
            TestItemAdditionalInit newItem = new TestItemAdditionalInit();
            newItem.Something = "a";
            dstore.SaveObject(newItem);

            Assert.IsTrue(newItem.id > 0);
            Assert.IsNotNull(dstore.LoadObject<TestItemAdditionalInit>(1));
        }

        [TestMethod]
        public virtual void Test_Additional_Init_Is_Called()
        {
            TestItemAdditionalInit newItem = new TestItemAdditionalInit();
            newItem.Something = "a";
            dstore.InsertObject(newItem);

            Assert.IsTrue(dstore.LoadObject(newItem));
            Assert.IsTrue(!string.IsNullOrEmpty(newItem.Something));
            Assert.IsTrue(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(newItem.Calculated == 15);
        }

        [TestMethod]
        public virtual void Test_Additional_Init_With_DataStore_Parm_Is_Called()
        {
            TestItemAdditionalInitWithParm newItem = new TestItemAdditionalInitWithParm();
            newItem.Something = "a";
            dstore.InsertObject(newItem);

            Assert.IsTrue(dstore.LoadObject(newItem));
            Assert.IsTrue(!string.IsNullOrEmpty(newItem.Something));
            Assert.IsTrue(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(newItem.Calculated > 0);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual void Test_Additional_Init_With_Bad_Parm_Fails()
        {
            TestItemAdditionalInitWithBadParm newItem = new TestItemAdditionalInitWithBadParm();
            newItem.Something = "a";
            dstore.InsertObject(newItem);

            Assert.IsTrue(dstore.LoadObject(newItem));
            Assert.IsTrue(!string.IsNullOrEmpty(newItem.Something));
            Assert.IsTrue(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(newItem.Calculated > 0);
        }

        [TestMethod]
        public virtual void Test_Can_Update_Item()
        {
            Test_Can_Insert_Object();
            IList<TestItem> items = dstore.LoadEntireTable<TestItem>().OrderBy(R => R.id).ToList();
            Assert.IsTrue(items.Count > 0);
            TestItem ti = items.Last();

            string value = ti.Something;
            ti.Something = Guid.NewGuid().ToString();

            Assert.IsTrue(dstore.UpdateObject(ti));

            dstore.LoadObject(ti);
            Assert.IsTrue(ti.Something != value);
        }

        [TestMethod]
        public virtual void QueryDataConstructorTest()
        {
            using (QueryData target = new QueryData())
            {
                Assert.IsFalse(target.QuerySuccessful);
            }
        }

        [TestMethod]
        public virtual void Test_Can_Parse_Object()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItem));
            Assert.IsNotNull(ti);

            Assert.IsNotNull(ti.DataFields);
            foreach (DataFieldInfo dfi in ti.DataFields)
            {
                Assert.IsTrue(dfi.LoadField);
                if (dfi.PrimaryKey) Assert.IsFalse(dfi.SetOnInsert);
                else Assert.IsTrue(dfi.SetOnInsert);
                Assert.IsTrue(!string.IsNullOrEmpty(dfi.FieldName));
                Assert.IsNotNull(dfi.Getter);
                Assert.IsNotNull(dfi.Setter);
            }


            Assert.IsFalse(ti.BypassValidation);
            Assert.IsNotNull(ti.AdditionalInit);
            Assert.IsTrue(ti.AdditionalInit.Count == 0);
        }

        [TestMethod]
        public virtual void Test_Can_parse_AdditionalInit()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemAdditionalInit));
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.AdditionalInit);
            Assert.IsTrue(ti.AdditionalInit.Count == 1);
        }

        [TestMethod]
        public virtual void Test_Can_parse_ByPass()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemBypassValidation));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.BypassValidation);
        }

        [TestMethod]
        public virtual void Test_Can_Parse_Foreign_Key()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemWithForeignKey));
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.DataFields);
            var target = ti.DataFields.Where(R => R.PrimaryKeyType != null).FirstOrDefault();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public virtual void Test_Can_Parse_Key_Attribute()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemWithKeyAttribute));
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.DataFields);
            var target = ti.DataFields.Where(R => R.PrimaryKey).FirstOrDefault();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public virtual void Test_Can_Parse_Table_Attribute()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemNewTableName));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.TableName.Contains("SomeNewTable"));

            ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemNewSchema));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.Schema.Contains("NewSchema"));
        }

        [TestMethod]
        public virtual void Test_Can_get_primary_Keys()
        {
            var nonPrimary = dstore.Connection.CommandGenerator.TypeParser.GetPrimaryKeys(typeof(TestItemWithKeyAttribute)).Where(R => R.PrimaryKey).FirstOrDefault();
            Assert.IsNotNull(nonPrimary);
        }

        [TestMethod]
        public virtual void Test_Can_Get_Type_Fields()
        {
            IEnumerable<DataFieldInfo> items = dstore.Connection.CommandGenerator.TypeParser.GetTypeFields(typeof(TestItem));
            Assert.IsNotNull(items);
            Assert.IsNotNull(items.Count() > 0);
        }

        [TestMethod]
        public virtual void Test_Ignored_Field_Is_Ignored()
        {
            IEnumerable<DataFieldInfo> fields = dstore.Connection.CommandGenerator.TypeParser.GetTypeFields(typeof(TestItemIgnoredField));
            Assert.IsTrue(fields != null);
            Assert.IsTrue(fields.Count() == 2);
            var ignored = fields.Where(R => R.PropertyName.Equals("Ignored", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(ignored == null);
        }

        [TestMethod]
        public virtual void Test_Can_Parse_View_Attribute()
        {
            dstore.Connection.CommandGenerator.TypeParser.GetTypeFields(typeof(ViewObject));

            IEnumerable<DataFieldInfo> fields = dstore.Connection.CommandGenerator.TypeParser.GetTypeFields(typeof(ViewObject));
            Assert.IsTrue(fields != null);
            Assert.IsTrue(fields.Count() == 1);
        }

        [TestMethod]
        public virtual void CanStartTransaction()
        {
            dstore.StartTransaction();
        }

        [TestMethod]
        public virtual void CanCommitTransaction()
        {
            dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItem));
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
            Assert.IsTrue(items.Count() == 1);
        }

        [TestMethod]
        public virtual void CanRollbackTransaction()
        {
            dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItem));
            TestItem ti = new TestItem()
            {
                Something = "foo"
            };

            dstore.InsertObject(ti);
            Assert.IsTrue(dstore.LoadObject<TestItem>(ti.id).Something == ti.Something);

            using (var context = dstore.StartTransaction())
            {
                ti.Something = "bar";
                context.Instance.UpdateObject(ti);
                context.Rollback();
            }

            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>().ToList();
            Assert.IsTrue(items.Count() == 1);
            Assert.IsTrue(items.First().Something == "foo");
        }

        [TestMethod]
        public virtual void Can_Do_Command_With_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems where Something = @query", new { query = "foo" });
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 1);
        }

        [TestMethod]
        public virtual void Can_Do_Command_Without_Parameter_Object()
        {
            dstore.InsertObject(new TestItem() { Something = "foo" });
            dstore.InsertObject(new TestItem() { Something = "bar" });
            dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems");
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 3);
        }

        [TestMethod]
        public virtual void Private_Additional_Init_Is_Called()
        {
            dstore.InsertObject(new TestItemPrivateInitMethod() { Name = "foo" });
            Assert.IsTrue(dstore.LoadObject<TestItemPrivateInitMethod>(1).Length == 3);
        }

        [TestMethod]
        public virtual void Additional_Init_Is_Called_When_On_Parent_Class()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            Assert.IsTrue(dstore.LoadObject<ChildClassWIithParentPrivateInitMethod>(1).Length == 3);
        }

        [TestMethod]
        public void Test_Update_Is_Closing_Connections()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                dstore.UpdateObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString(), ID = 1 });
            }
        }

        [TestMethod]
        public void Test_Insert_Is_Closing_Connections()
        {
            dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString() });
            }
        }

        [TestMethod]
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
