using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System.Collections.Concurrent;
using Tests.DataObjects;

namespace Tests.Tests
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
            dstore.InitDataStore().Wait();
        }

        [TestMethod]
        public virtual async Task Test_Can_Insert_Many_Items()
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

            await dstore.InsertObjects(items);

            List<TestBulkItem> loaded = dstore.LoadEntireTable<TestBulkItem>().ToBlockingEnumerable().ToList();
            Assert.IsTrue(loaded.Count == numItems);
        }

        [TestMethod]
        public virtual async Task Test_Can_Load_Object_With_Enums()
        {
            Assert.IsTrue(await dstore.InsertObject(new TestItemWithEnum()
            {
                ValueOne = Data.Var1,
                ValueTwo = Data.Var2,
                AnotherValue = 12
            }));

            TestItemWithEnum loaded = await dstore.LoadObject<TestItemWithEnum>(1);
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

            List<TestBulkItem> loaded = dstore.LoadEntireTable<TestBulkItem>().ToBlockingEnumerable().ToList();
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
        public virtual async Task Test_Can_Load_Object()
        {
            await Test_Can_Insert_Object();
            TestItem ti = new TestItem();
            ti.id = 1;

            Assert.IsTrue(await dstore.LoadObject(ti));
            Assert.IsTrue(!string.IsNullOrEmpty(ti.Something));
            Assert.IsTrue(ti.Something.Equals("SomethingNew", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public virtual async Task Test_Can_Load_Object_By_Key_Templeted()
        {
            TestItem newObject = new TestItem();
            newObject.Something = "A Test String";
            Assert.IsTrue(await dstore.InsertObject(newObject));

            TestItem ti = await dstore.LoadObject<TestItem>(newObject.id);

            Assert.IsTrue(await dstore.LoadObject(ti));
            Assert.IsTrue(!string.IsNullOrEmpty(ti.Something));
            Assert.IsTrue(ti.id == 1);
            Assert.IsTrue(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));

            ti = await dstore.LoadObject<TestItem>(1);

            Assert.IsTrue(await dstore.LoadObject(ti));
            Assert.IsTrue(!string.IsNullOrEmpty(ti.Something));
            Assert.IsTrue(ti.id == 1);
            Assert.IsTrue(ti.Something.Equals("A Test String", StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        public virtual async Task Test_Can_Insert_Object()
        {
            TestItem ti = new TestItem();
            ti.Something = "SomethingNew";
            Assert.IsTrue(await dstore.InsertObject(ti));
        }

        [TestMethod]
        public virtual async Task Test_Get_Key_For_Item()
        {
            TestItem ti = new TestItem();
            ti.id = 1;

            var iKey = await dstore.GetKeyForItemType(typeof(TestItem), ti);
            Assert.IsTrue(((int)iKey) == 1);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual async Task Test_Get_Key_Fails_For_Many_Keys()
        {
            await dstore.GetKeyForItemType(typeof(TestItemTwoKeys), null);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual async Task Test_Get_Key_Fails_For_No_Keys()
        {
            await dstore.GetKeyForItemType(typeof(TestItemNoKey), null);
        }

        [TestMethod]
        public virtual async Task Test_Can_Delete_Item()
        {
            await Test_Can_Insert_Object();
            List<TestItem> items = dstore.LoadEntireTable<TestItem>().ToBlockingEnumerable().OrderBy(R => R.id).ToList();
            Assert.IsTrue(await dstore.DeleteObject(items.Last()));
            Assert.IsNull(await dstore.LoadObject<TestItem>(items.Last().id));
        }

        [TestMethod]
        public virtual async Task Test_Can_Load_Entire_Table()
        {
            var items = dstore.LoadEntireTable(typeof(TestItem));
            Assert.IsTrue(items != null);

            await foreach (object o in items)
            {
                Assert.IsTrue(o != null);
            }
        }

        [TestMethod]
        public virtual async Task Test_Can_Load_Entire_Table_Templeted()
        {
            var items = dstore.LoadEntireTable<TestItem>();
            Assert.IsTrue(items != null);

            await foreach (TestItem o in items)
            {
                Assert.IsTrue(o != null);
                Assert.IsTrue(o.id != 0);
                Assert.IsTrue(!string.IsNullOrEmpty(o.Something));
            }
        }

        [TestMethod]
        public virtual async Task Test_Is_new()
        {
            await Test_Can_Insert_Object();
            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>().ToBlockingEnumerable();
            Assert.IsTrue(!await dstore.IsNew(items.ElementAt(0)));
        }

        [TestMethod]
        public virtual async Task Test_Can_Insert_Multiple_Items()
        {
            List<TestItem> items = [];

            for (int i = 0; i < 10; i++)
                items.Add(new TestItem() { Something = Guid.NewGuid().ToString() });

            Assert.IsTrue(await dstore.InsertObjects(items));
        }

        [TestMethod]
        public virtual async Task Test_Can_Save_Object()
        {
            TestItemAdditionalInit newItem = new TestItemAdditionalInit();
            newItem.Something = "a";
            await dstore.SaveObject(newItem);

            Assert.IsTrue(newItem.id > 0);
            Assert.IsNotNull(await dstore.LoadObject<TestItemAdditionalInit>(1));
        }

        [TestMethod]
        public virtual async Task Test_Additional_Init_Is_Called()
        {
            TestItemAdditionalInit newItem = new TestItemAdditionalInit();
            newItem.Something = "a";
            await dstore.InsertObject(newItem);

            Assert.IsTrue(await dstore.LoadObject(newItem));
            Assert.IsTrue(!string.IsNullOrEmpty(newItem.Something));
            Assert.IsTrue(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(newItem.Calculated == 15);
        }

        [TestMethod]
        public virtual async Task Test_Additional_Init_With_DataStore_Parm_Is_Called()
        {
            TestItemAdditionalInitWithParm newItem = new TestItemAdditionalInitWithParm();
            newItem.Something = "a";
            await dstore.InsertObject(newItem);

            Assert.IsTrue(await dstore.LoadObject(newItem));
            Assert.IsTrue(!string.IsNullOrEmpty(newItem.Something));
            Assert.IsTrue(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(newItem.Calculated > 0);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual async Task Test_Additional_Init_With_Bad_Parm_Fails()
        {
            TestItemAdditionalInitWithBadParm newItem = new TestItemAdditionalInitWithBadParm();
            newItem.Something = "a";
            await dstore.InsertObject(newItem);

            Assert.IsTrue(await dstore.LoadObject(newItem));
            Assert.IsTrue(!string.IsNullOrEmpty(newItem.Something));
            Assert.IsTrue(newItem.Something.Equals("a", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(newItem.Calculated > 0);
        }

        [TestMethod]
        public virtual async Task Test_Can_Update_Item()
        {
            await Test_Can_Insert_Object();
            IList<TestItem> items = dstore.LoadEntireTable<TestItem>().ToBlockingEnumerable().OrderBy(R => R.id).ToList();
            Assert.IsTrue(items.Count > 0);
            TestItem ti = items.Last();

            string value = ti.Something;
            ti.Something = Guid.NewGuid().ToString();

            Assert.IsTrue(await dstore.UpdateObject(ti));

            await dstore.LoadObject(ti);
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
        public virtual async Task Test_Can_Parse_Object()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItem));
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
        public virtual async Task Test_Can_parse_AdditionalInit()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemAdditionalInit));
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.AdditionalInit);
            Assert.IsTrue(ti.AdditionalInit.Count == 1);
        }

        [TestMethod]
        public virtual async Task Test_Can_parse_ByPass()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemBypassValidation));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.BypassValidation);
        }

        [TestMethod]
        public virtual async Task Test_Can_Parse_Foreign_Key()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemWithForeignKey));
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.DataFields);
            var target = ti.DataFields.Where(R => R.PrimaryKeyType != null).FirstOrDefault();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public virtual async Task Test_Can_Parse_Key_Attribute()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemWithKeyAttribute));
            Assert.IsNotNull(ti);
            Assert.IsNotNull(ti.DataFields);
            var target = ti.DataFields.Where(R => R.PrimaryKey).FirstOrDefault();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public virtual async Task Test_Can_Parse_Table_Attribute()
        {
            DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemNewTableName));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.TableName.Contains("SomeNewTable"));

            ti = await dstore.TypeParser.GetTypeInfo(typeof(TestItemNewSchema));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.Schema.Contains("NewSchema"));
        }

        [TestMethod]
        public virtual async Task Test_Can_get_primary_Keys()
        {
            var nonPrimary = (await dstore.TypeParser.GetPrimaryKeys(typeof(TestItemWithKeyAttribute))).Where(R => R.PrimaryKey).FirstOrDefault();
            Assert.IsNotNull(nonPrimary);
        }

        [TestMethod]
        public virtual async Task Test_Can_Get_Type_Fields()
        {
            IEnumerable<DataFieldInfo> items = await dstore.TypeParser.GetTypeFields(typeof(TestItem));
            Assert.IsNotNull(items);
            Assert.IsNotNull(items.Count() > 0);
        }

        [TestMethod]
        public virtual async Task Test_Ignored_Field_Is_Ignored()
        {
            IEnumerable<DataFieldInfo> fields = await dstore.TypeParser.GetTypeFields(typeof(TestItemIgnoredField));
            Assert.IsTrue(fields != null);
            Assert.IsTrue(fields.Count() == 2);
            var ignored = fields.Where(R => R.PropertyName.Equals("Ignored", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(ignored == null);
        }

        [TestMethod]
        public virtual async Task Test_Can_Parse_View_Attribute()
        {
            await dstore.TypeParser.GetTypeFields(typeof(ViewObject));

            IEnumerable<DataFieldInfo> fields = await dstore.TypeParser.GetTypeFields(typeof(ViewObject));
            Assert.IsTrue(fields != null);
            Assert.IsTrue(fields.Count() == 1);
        }

        [TestMethod]
        public virtual void CanStartTransaction()
        {
            dstore.StartTransaction();
        }

        [TestMethod]
        public virtual async Task CanCommitTransaction()
        {
            await dstore.TypeParser.GetTypeInfo(typeof(TestItem));
            TestItem ti;

            try
            {
                using (var context = dstore.StartTransaction())
                {
                    ti = new TestItem()
                    {
                        Something = "foo"
                    };

                    await context.Instance.InsertObject(ti);
                    context.Commit();
                }
            }
            catch (Exception ex)
            {
                Assert.IsTrue(false);
            }
            
            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>().ToBlockingEnumerable();
            Assert.IsTrue(items.Count() == 1);
        }

        [TestMethod]
        public virtual async Task CanRollbackTransaction()
        {
            await dstore.TypeParser.GetTypeInfo(typeof(TestItem));
            TestItem ti = new TestItem()
            {
                Something = "foo"
            };

            await dstore.InsertObject(ti);
            var item = await dstore.LoadObject<TestItem>(ti.id);

            Assert.IsTrue(item.Something == ti.Something);

            using (var context = dstore.StartTransaction())
            {
                ti.Something = "bar";
                await context.Instance.UpdateObject(ti);
                context.Rollback();
            }

            IEnumerable<TestItem> items = dstore.LoadEntireTable<TestItem>().ToBlockingEnumerable().ToList();
            Assert.IsTrue(items.Count() == 1);
            Assert.IsTrue(items.First().Something == "foo");
        }

        [TestMethod]
        public virtual async Task Can_Do_Command_With_Parameter_Object()
        {
            await dstore.InsertObject(new TestItem() { Something = "foo" });
            await dstore.InsertObject(new TestItem() { Something = "bar" });
            await dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems where Something = @query", new { query = "foo" }).ToBlockingEnumerable();
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 1);
        }

        [TestMethod]
        public virtual async Task Can_Do_Command_Without_Parameter_Object()
        {
            await dstore.InsertObject(new TestItem() { Something = "foo" });
            await dstore.InsertObject(new TestItem() { Something = "bar" });
            await dstore.InsertObject(new TestItem() { Something = "foobar" });

            var items = dstore.GetCommand<TestItem>().ExecuteQuery("select * from TestItems").ToBlockingEnumerable();
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 3);
        }

        [TestMethod]
        public virtual async Task Private_Additional_Init_Is_Called()
        {
            await dstore.InsertObject(new TestItemPrivateInitMethod() { Name = "foo" });
            var item = await dstore.LoadObject<TestItemPrivateInitMethod>(1);
            Assert.IsTrue(item.Length == 3);
        }

        [TestMethod]
        public virtual async Task Additional_Init_Is_Called_When_On_Parent_Class()
        {
            await dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            var item = await dstore.LoadObject<ChildClassWIithParentPrivateInitMethod>(1);
            Assert.IsTrue(item.Length == 3);
        }

        [TestMethod]
        public async Task Test_Update_Is_Closing_Connections()
        {
            await dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                await dstore.UpdateObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString(), ID = 1 });
            }
        }

        [TestMethod]
        public async Task Test_Insert_Is_Closing_Connections()
        {
            await dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                await dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString() });
            }
        }

        [TestMethod]
        public async Task Test_Transaction_Is_Closing_Connections()
        {
            await dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });
            for (int i = 0; i < 5000; i++)
            {
                using (TransactionContext ctx = dstore.StartTransaction())
                {
                    await ctx.Instance.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = i.ToString() });
                    ctx.Commit();
                }
            }
        }

        [TestMethod]
        public async Task Test_Read_Is_Thread_Safe()
        {
            for (var i = 0; i < 50; i++)
                await dstore.InsertObject(new ChildClassWIithParentPrivateInitMethod() { Name = "foo" });

            ConcurrentBag<ChildClassWIithParentPrivateInitMethod> infos = new ConcurrentBag<ChildClassWIithParentPrivateInitMethod>();
            List<Thread> threads = new List<Thread>();
            int numThreads = 50;
            int numGets = 20;

            for (var i = 0; i < numThreads; i++)
            {
                threads.Add(new Thread(() =>
                {
                    for (var x = 0; x < numGets; x++)
                        dstore.LoadEntireTable<ChildClassWIithParentPrivateInitMethod>().ToBlockingEnumerable().ToList().ForEach(r => infos.Add(r));
                }));
            }

            threads.ForEach(r => r.Start());

            while (threads.Count(r => r.ThreadState == ThreadState.Running) > 0)
                Thread.Sleep(250);

            foreach (ChildClassWIithParentPrivateInitMethod item in infos)
                Assert.IsTrue(item.ID != 0);
        }
    }
}
