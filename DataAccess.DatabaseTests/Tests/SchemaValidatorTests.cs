using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using DataAccess.Core;
using DataAccess.Core.Interfaces;
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
                dstore.InitDataStore();
        }

        [TestMethod]
        public virtual void Test_Can_Get_Tables()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
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
        public virtual void Test_Can_Add_Table_With_Schema()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(ItemWithSchema));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
        }

        [TestMethod]
        public virtual void Test_Can_Add_Table_Two_Key()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
        }

        [TestMethod]
        public virtual void Test_Can_Add_Table_One_Key()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemNewTableName));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
        }

        [TestMethod]
        public virtual void Test_Can_Add_Table_With_Foreign_Key()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemWithForeignKey));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
        }

        [TestMethod]
        public virtual void Test_Can_Add_Column_To_Table()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 4);
        }

        [TestMethod]
        public virtual void Test_Will_Not_Add_Column_To_Table_With_Option_Off()
        {
            dstore.SchemaValidator.CanAddColumns = false;
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };
            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 3);
        }

        [TestMethod]
        public virtual void Test_Can_Add_Column_To_Table_Foreign_Key()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneForeign));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 4);
        }

        [TestMethod]
        public virtual void Test_Can_RemoveColumn()
        {
            dstore.SchemaValidator.CanRemoveColumns = true;
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsMinusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 2);
        }

        [TestMethod]
        public virtual void Test_Will_Not_Remove_Column_With_Option_Off()
        {
            dstore.SchemaValidator.CanRemoveColumns = false;
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sende, args) =>
            {
                Assert.IsTrue(false);
            };

            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsMinusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 3);
        }

        [TestMethod]
        public virtual void Test_Can_Add_Foreign_Key_When_Types_Are_Different_But_Can_Convert()
        {
            DatabaseTypeInfo pT = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(pT != null);

            DatabaseTypeInfo fT = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemForeignKeyWithString));
            Assert.IsTrue(fT != null);
        }

        [TestMethod]
        public virtual void Test_Can_Modify_Column_Type()
        {
            int columnsModified = 0;
            DatabaseTypeInfo ti1 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.IsTrue(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.Date = "11/20/2010";
            tipk.ID = Guid.NewGuid().ToString();
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
                {
                    columnsModified++;
                };

            DatabaseTypeInfo ti2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.IsTrue(ti2 != null);
            Assert.IsTrue(columnsModified > 0);
        }

        [TestMethod]
        public virtual void Test_Will_Not_Modify_Column_With_Option_Off()
        {
            dstore.SchemaValidator.CanUpdateColumns = false;
            DatabaseTypeInfo ti1 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKey));
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

            DatabaseTypeInfo ti2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.IsTrue(ti2 != null);
        }

        [TestMethod]
        public virtual void Test_Can_Use_Reservered_Words_In_Names()
        {
            dstore.InsertObject(new Groups() { Group = "First" });
        }

        [TestMethod]
        public virtual void Test_Can_Do_On_Table_Create()
        {
            IAsyncEnumerable<TestItemOnTableCreated> items = dstore.LoadEntireTable<TestItemOnTableCreated>();
            Assert.IsTrue(items.ToBlockingEnumerable().Count() > 0);
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual void Test_Exception_When_On_Table_Create_Is_Not_Static()
        {
            IAsyncEnumerable<TestItemOnTableCreatedNonStatic> items = dstore.LoadEntireTable<TestItemOnTableCreatedNonStatic>();
            Assert.IsTrue(items.ToBlockingEnumerable().Count() > 0);
        }

        [TestMethod]
        public virtual void Test_Can_Create_Table_With_No_Primary_Key()
        {
            IEnumerable<ClassNoPrimaryKey> items = dstore.LoadEntireTable<ClassNoPrimaryKey>().ToBlockingEnumerable();
        }

        [TestMethod]
        public virtual void Test_DB_Types_Come_Back_Right()
        {
            dstore.LoadEntireTable<DBTypeTestObject>();
            IEnumerable<DBObject> objects = dstore.SchemaValidator.TableValidator.GetObjects(true);
            Assert.IsTrue(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("DBTypeTestObjects", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore = GetDataStore();
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };

            dstore.LoadEntireTable<DBTypeTestObject>();
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public void Test_Notify_Validator_Notifies_Of_Missing_Table()
        {
            dstore.SchemaValidator = new NotifyValidator(dstore);
            dstore.InsertObject(new TestItem() { });
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual void Test_Notify_Validator_Notifies_Of_Missing_Column()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            //column is there, lets change the validator and try to add a column...
            dstore.SchemaValidator = new NotifyValidator(dstore);
            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));
        }

        [TestMethod, ExpectedException(typeof(DataStoreException))]
        public virtual void Test_View_Validator_Notifies_Of_Missing_Columns()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(ItemWithSchema));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();

            //need to check the column with a new item + one column
            dstore.SchemaValidator = new NotifyValidator(dstore);

            //make sure the object is reparsed
            dstore.Connection.CommandGenerator.TypeParser.ClearCache();
            dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(ItemWithSchemaAnotherColumn));
        }

        [TestMethod]
        public virtual void Test_TransactionContext_Has_Same_Schema_Validator()
        {
            TransactionContext ctx = dstore.StartTransaction();
            Assert.IsTrue(ctx.Instance.SchemaValidator.GetType() == dstore.SchemaValidator.GetType());
        }

        [TestMethod]
        public virtual void Test_Can_Add_Nullable()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemNullable));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
        }

        [TestMethod]
        public virtual void Test_Can_Add_Nullable_Column_To_Table()
        {
            DatabaseTypeInfo ti = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.Columns.Count == 3);

            DatabaseTypeInfo t2 = dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneNullable));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(inserted2 != null);
            Assert.IsTrue(inserted2.Columns.Count == 4);
        }

        [TestMethod]
        public virtual void Test_Schema_Validator_Doesnt_Try_To_Modify_Columns_When_Not_Needed()
        {
            dstore.LoadEntireTable<TestObjectManyColumns>();
            IEnumerable<DBObject> objects = dstore.SchemaValidator.TableValidator.GetObjects(true);
            Assert.IsTrue(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("TestObjectManyColumns", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.IsTrue(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore.Connection.CommandGenerator.TypeParser.ClearCache();

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.IsTrue(false);
            };

            dstore.LoadEntireTable<TestObjectManyColumns>();
        }

        [TestMethod]
        public virtual void Test_Honors_Field_Length()
        {
            Assert.IsTrue(dstore.Connection.CommandGenerator.TypeParser.GetTypeInfo(typeof(TestItemSmallString)).DataFields[1].FieldLength == 5);
            dstore.InsertObject(new TestItemSmallString() { SmallString = "hi" }); //should work with no error
        }
    }
}

