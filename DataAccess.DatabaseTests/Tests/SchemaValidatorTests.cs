using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core;
using System.Data;
using DataAccess.Core.Schema;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DataAccess.DatabaseTests.Tests
{
    [TestClass]
    public abstract class SchemaValidatorTests
    {
        protected IDataStore dstore;
        public abstract IDataStore GetDataStore();

        public SchemaValidatorTests()
        {
            Init();
        }

        public void Init()
        {
            dstore = GetDataStore();
            if (dstore != null)
                Task.WaitAll(dstore.InitDataStore());
        }

        [TestMethod]
        public virtual async Task Test_Can_Get_Tables()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(tables.Count() > 0);
            foreach (DBObject t in tables)
            {
                Assert.IsTrue(t != null);
                Assert.IsTrue(t.Name != "");
                Assert.IsTrue(t.Schema != "");
                Assert.IsNotNull(t.Columns);
                Assert.IsTrue(t.Columns.Count > 0);

                foreach (Column c in t.Columns)
                {
                    Assert.IsTrue(c != null);
                    Assert.IsTrue(c.Name != "");
                }
            }
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Table_With_Schema()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(ItemWithSchema));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Table_Two_Key()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Table_One_Key()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemNewTableName));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Table_With_Foreign_Key()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemWithForeignKey));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Column_To_Table()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));

            tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 4);
        }

        [TestMethod]
        public virtual async Task Test_Will_Not_Add_Column_To_Table_With_Option_Off()
        {
            dstore.SchemaValidator.CanAddColumns = false;
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };
            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));

            tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 3);
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Column_To_Table_Foreign_Key()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneForeign));

            tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 4);
        }

        [TestMethod]
        public virtual async Task Test_Can_RemoveColumn()
        {
            dstore.SchemaValidator.CanRemoveColumns = true;
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsMinusOne));

            tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 2);
        }

        [TestMethod]
        public virtual async Task Test_Will_Not_Remove_Column_With_Option_Off()
        {
            dstore.SchemaValidator.CanRemoveColumns = false;
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sende, args) =>
            {
                Assert.IsTrue(false);
            };

            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsMinusOne));

            tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 3);
        }

        [TestMethod]
        public virtual void Test_Can_Add_Foreign_Key_When_Types_Are_Different_But_Can_Convert()
        {
            DatabaseTypeInfo pT = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(pT != null);

            DatabaseTypeInfo fT = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemForeignKeyWithString));
            Assert.IsTrue(fT != null);
        }

        [TestMethod]
        public virtual void Test_Can_Modify_Column_Type()
        {
            int columnsModified = 0;
            DatabaseTypeInfo ti1 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.Date = "11/20/2010";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
                {
                    columnsModified++;
                };

            DatabaseTypeInfo ti2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.IsTrue(ti2 != null);
            Assert.IsTrue(columnsModified > 0);
        }

        [TestMethod]
        public virtual void Test_Will_Not_Modify_Column_With_Option_Off()
        {
            dstore.SchemaValidator.CanUpdateColumns = false;
            DatabaseTypeInfo ti1 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.ID = "1";
            tipk.Date = "11/20/2010";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };

            DatabaseTypeInfo ti2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.IsTrue(ti2 != null);
        }

        [TestMethod]
        public virtual void Test_Can_Use_Reservered_Words_In_Names()
        {
            dstore.InsertObject(new Groups() { Group = "First" });
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_On_Table_Create()
        {
            IEnumerable<TestItemOnTableCreated> items = await dstore.LoadEntireTable<TestItemOnTableCreated>();
            Assert.IsTrue(items.Count() > 0);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual async Task Test_Exception_When_On_Table_Create_Is_Not_Static()
        {
            IEnumerable<TestItemOnTableCreatedNonStatic> items = await dstore.LoadEntireTable<TestItemOnTableCreatedNonStatic>();
            Assert.IsTrue(items.Count() > 0);
        }

        [TestMethod]
        public virtual async Task Test_Can_Create_Table_With_No_Primary_Key()
        {
            IEnumerable<ClassNoPrimaryKey> items = await dstore.LoadEntireTable<ClassNoPrimaryKey>();
        }

        [TestMethod]
        public virtual async Task Test_DB_Types_Come_Back_Right()
        {
            await dstore.LoadEntireTable<DBTypeTestObject>();
            IEnumerable<DBObject> objects = await dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("DBTypeTestObjects", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore = GetDataStore();
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };

            await dstore.LoadEntireTable<DBTypeTestObject>();
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public async Task Test_Notify_Validator_Notifies_Of_Missing_Table()
        {
            dstore.SchemaValidator = new NotifyValidator(dstore);
            await dstore.InsertObject(new TestItem() { });
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual async Task Test_Notify_Validator_Notifies_Of_Missing_Column()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            //column is there, lets change the validator and try to add a column...
            dstore.SchemaValidator = new NotifyValidator(dstore);
            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual async Task Test_View_Validator_Notifies_Of_Missing_Columns()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(ItemWithSchema));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();

            //need to check the column with a new item + one column
            dstore.SchemaValidator = new NotifyValidator(dstore);

            //make sure the object is reparsed
            dstore.TypeInformationParser.Cache.ClearCache();
            dstore.TypeInformationParser.GetTypeInfo(typeof(ItemWithSchemaAnotherColumn));
        }

        [TestMethod]
        public virtual void Test_TransactionContext_Has_Same_Schema_Validator()
        {
            TransactionContext ctx = dstore.StartTransaction();
            Assert.IsTrue(ctx.Instance.SchemaValidator.GetType() == dstore.SchemaValidator.GetType());
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Nullable()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemNullable));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
        }

        [TestMethod]
        public virtual async Task Test_Can_Add_Nullable_Column_To_Table()
        {
            DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneNullable));

            tables = await dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 4);
        }

        [TestMethod]
        public virtual async Task Test_Schema_Validator_Doesnt_Try_To_Modify_Columns_When_Not_Needed()
        {
            await dstore.LoadEntireTable<TestObjectManyColumns>();
            IEnumerable<DBObject> objects = await dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.IsTrue(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("TestObjectManyColumns", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore.TypeInformationParser.Cache.ClearCache();

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };

            await dstore.LoadEntireTable<TestObjectManyColumns>();
        }

        [TestMethod]
        public virtual void Test_Honors_Field_Length()
        {
            Assert.IsTrue(dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemSmallString)).DataFields[1].FieldLength == 5);

            dstore.InsertObject(new TestItemSmallString() { SmallString = "hi" }); //should work with no error
        }
    }
}

