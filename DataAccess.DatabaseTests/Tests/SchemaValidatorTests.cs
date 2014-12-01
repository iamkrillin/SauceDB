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
using Xunit;
using System.Threading;

namespace DataAccess.DatabaseTests.Tests
{
    public abstract class SchemaValidatorTests
    {
        protected IDataStore dstore;
        public abstract IDataStore GetDataStore();

        public SchemaValidatorTests()
        {
            Init();
        }

        private void Init()
        {
            dstore = GetDataStore();
            if(dstore != null)
                dstore.InitDataStore();
        }

        public virtual void Cleanup()
        {
            Init();
        }

        [Fact]
        public virtual void Test_Can_Get_Tables()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            Assert.True(tables.Count() > 0);
            foreach (DBObject t in tables)
            {
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
        public virtual void Test_Can_Add_Table_With_Schema()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(ItemWithSchema));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
        }

        [Fact]
        public virtual void Test_Can_Add_Table_Two_Key()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemTwoKeys));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
        }

        [Fact]
        public virtual void Test_Can_Add_Table_One_Key()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemNewTableName));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
        }

        [Fact]
        public virtual void Test_Can_Add_Table_With_Foreign_Key()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemWithForeignKey));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
        }

        [Fact]
        public virtual void Test_Can_Add_Table_With_Default_Value()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemDefaultValue));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);

            TestItemDefaultValue newItem = new TestItemDefaultValue();
            newItem.AnotherField = Guid.NewGuid().ToString();
            dstore.InsertObject(newItem);
            dstore.LoadObject(newItem);
            Assert.True(!string.IsNullOrEmpty(newItem.Something));
        }

        [Fact]
        public virtual void Test_Can_Add_Column_To_Table()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 4);
        }

        [Fact]
        public virtual void Test_Will_Not_Add_Column_To_Table_With_Option_Off()
        {
            dstore.SchemaValidator.CanAddColumns = false;
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.True(false);
            };
            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 3);
        }

        [Fact]
        public virtual void Test_Can_Add_Column_To_Table_Default()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneWithDefault));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 4);
        }

        [Fact]
        public virtual void Test_Can_Add_Column_To_Table_Foreign_Key()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneForeign));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 4);
        }

        [Fact]
        public virtual void Test_Can_RemoveColumn()
        {
            dstore.SchemaValidator.CanRemoveColumns = true;
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsMinusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 2);
        }

        [Fact]
        public virtual void Test_Will_Not_Remove_Column_With_Option_Off()
        {
            dstore.SchemaValidator.CanRemoveColumns = false;
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            dstore.SchemaValidator.TableValidator.OnObjectModified += (sende, args) =>
            {
                Assert.True(false);
            };

            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsMinusOne));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 3);
        }

        [Fact]
        public virtual void Test_Can_Add_Foreign_Key_When_Types_Are_Different_But_Can_Convert()
        {
            TypeInfo pT = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.True(pT != null);

            TypeInfo fT = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemForeignKeyWithString));
            Assert.True(fT != null);
        }

        [Fact]
        public virtual void Test_Can_Modify_Column_Type()
        {
            int columnsModified = 0;
            TypeInfo ti1 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.True(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.Date = "11/20/2010";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
                {
                    columnsModified++;
                };

            TypeInfo ti2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.True(ti2 != null);
            Assert.True(columnsModified > 0);
        }

        [Fact]
        public virtual void Test_Will_Not_Modify_Column_With_Option_Off()
        {
            dstore.SchemaValidator.CanUpdateColumns = false;
            TypeInfo ti1 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKey));
            Assert.True(ti1 != null);

            TestItemPrimaryKey tipk = new TestItemPrimaryKey();
            tipk.ID = "1";
            tipk.Date = "11/20/2010";
            tipk.Name = "Hello";
            dstore.InsertObject(tipk);
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.True(false);
            };

            TypeInfo ti2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemPrimaryKeyDateFieldDifferentType));
            Assert.True(ti2 != null);
        }

        [Fact]
        public virtual void Test_Can_Use_Reservered_Words_In_Names()
        {
            dstore.InsertObject(new Groups() { Group = "First" });
        }

        [Fact]
        public virtual void Test_Can_Modify_Column_And_Keep_Default_Value()
        {
            dstore.InsertObject(new TestItemDefaultValue() { AnotherField = "First" });
            dstore.InsertObject(new TestItemDefaultValueDifferntFieldType() { AnotherField = "Second" });

            dstore.LoadEntireTable<TestItemDefaultValue>().ToList().ForEach(R => Assert.True(R.Something.Equals("SomeDefaultValue", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Fact]
        public virtual void Test_Can_Do_On_Table_Create()
        {
            IEnumerable<TestItemOnTableCreated> items = dstore.LoadEntireTable<TestItemOnTableCreated>();
            Assert.True(items.Count() > 0);
        }

        [Fact]
        public virtual void Test_Exception_When_On_Table_Create_Is_Not_Static()
        {
            Assert.Throws(typeof(DataStoreException), () =>
                {
                    IEnumerable<TestItemOnTableCreatedNonStatic> items = dstore.LoadEntireTable<TestItemOnTableCreatedNonStatic>();
                    Assert.True(items.Count() > 0);
                });
        }

        [Fact]
        public virtual void Test_Can_Create_Table_With_No_Primary_Key()
        {
            IEnumerable<ClassNoPrimaryKey> items = dstore.LoadEntireTable<ClassNoPrimaryKey>();
        }

        [Fact]
        public virtual void Test_DB_Types_Come_Back_Right()
        {
            dstore.LoadEntireTable<DBTypeTestObject>();
            IEnumerable<DBObject> objects = dstore.SchemaValidator.TableValidator.GetObjects(true);
            Assert.True(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("DBTypeTestObjects", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore = GetDataStore();
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.True(false);
            };

            dstore.LoadEntireTable<DBTypeTestObject>();

        }

        [Fact]
        public void Test_Notify_Validator_Notifies_Of_Missing_Table()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                dstore.SchemaValidator = new NotifyValidator(dstore);
                dstore.InsertObject(new TestItemDefaultValue() { AnotherField = "First" });
            });
        }

        [Fact]
        public virtual void Test_Notify_Validator_Notifies_Of_Missing_Column()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
                IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
                DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                Assert.True(inserted != null);
                Assert.True(inserted.Columns.Count == 3);

                //column is there, lets change the validator and try to add a column...
                dstore.SchemaValidator = new NotifyValidator(dstore);
                TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOne));
            });
        }

        [Fact]
        public virtual void Test_View_Validator_Notifies_Of_Missing_Columns()
        {
            Assert.Throws(typeof(DataStoreException), () =>
            {
                TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(ItemWithSchema));
                IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();

                //need to check the column with a new item + one column
                dstore.SchemaValidator = new NotifyValidator(dstore);

                //make sure the object is reparsed
                dstore.TypeInformationParser.Cache.ClearCache();
                dstore.TypeInformationParser.GetTypeInfo(typeof(ItemWithSchemaAnotherColumn));
            });
        }

        [Fact]
        public virtual void Test_TransactionContext_Has_Same_Schema_Validator()
        {
            TransactionContext ctx = dstore.StartTransaction();
            Assert.True(ctx.Instance.SchemaValidator.GetType() == dstore.SchemaValidator.GetType());
        }

        [Fact]
        public virtual void Test_Can_Add_Nullable()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemNullable));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
        }

        [Fact]
        public virtual void Test_Can_Add_Nullable_Column_To_Table()
        {
            TypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFields));
            IEnumerable<DBObject> tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted != null);
            Assert.True(inserted.Columns.Count == 3);

            TypeInfo t2 = dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemThreeFieldsPlusOneNullable));

            tables = dstore.SchemaValidator.TableValidator.GetObjects();
            DBObject inserted2 = tables.Where(R => R.Name.Equals(ti.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(inserted2 != null);
            Assert.True(inserted2.Columns.Count == 4);
        }

        [Fact]
        public virtual void Test_Schema_Validator_Doesnt_Try_To_Modify_Columns_When_Not_Needed()
        {
            dstore.LoadEntireTable<TestObjectManyColumns>();
            IEnumerable<DBObject> objects = dstore.SchemaValidator.TableValidator.GetObjects(true);
            Assert.True(objects != null);
            DBObject obj = objects.Where(r => r.Name.Equals("TestObjectManyColumns", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            Assert.True(obj != null);

            //to test this.. I'm going to pass the object back through the validator.... no columns should require modification
            dstore.TypeInformationParser.Cache.ClearCache();
            
            dstore.SchemaValidator.TableValidator.OnObjectModified += (sender, args) =>
            {
                Assert.True(false);
            };

            dstore.LoadEntireTable<TestObjectManyColumns>();
        }

        [Fact]
        public virtual void Test_Honors_Field_Length()
        {
            Assert.True(dstore.TypeInformationParser.GetTypeInfo(typeof(TestItemSmallString)).DataFields[1].FieldLength == 5);

            dstore.InsertObject(new TestItemSmallString() { SmallString = "hi" }); //should work with no error
        }
    }
}

