using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.Core;
using DataAccess.Core.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Tests.DataObjects;

namespace Tests.Tests
{
    [TestClass]
    public abstract class LinqTests
    {
        protected IDataStore dStore;
        public abstract IDataStore GetDataStore();

        public LinqTests()
        {
            dStore = GetDataStore();
            dStore.InitDataStore().Wait();
        }

        [TestMethod]
        public virtual async Task Test_Where_Take()
        {
            for (int i = 0; i < 10; i++)
                await dStore.InsertObject(new TestItem() { Something = "foo" });
            
            var result = dStore.Query<TestItem>().Where(R => R.Something == "foo").Take(2).ToList();
            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod]
        public virtual async Task Test_Can_Return_Count()
        {
            for (int i = 0; i < 10; i++)
                await dStore.InsertObject(new TestItem() { Something = "foo" });

            var result = dStore.Query<TestItem>().Where(R => R.Something == "foo").Count();
            Assert.IsTrue(result == 10);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Join_And_Return_Aggregate_Object()
        {
            TestItemPrimaryKey newItem = new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() };
            await dStore.InsertObject(newItem);
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });

            var result = from i in dStore.Query<TestItemForeignKeyWithString>()
                         join x in dStore.Query<TestItemPrimaryKey>() on i.FKeyField equals x.ID
                         select new
                         {
                             i.ID,
                             x.Name,
                             x.Date
                         };

            result.ToList().ForEach(R => Assert.IsTrue(!string.IsNullOrEmpty(R.Name)));
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Join_And_Return_Compound_Object()
        {
            TestItemPrimaryKey newItem = new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() };
            await dStore.InsertObject(newItem);
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemForeignKeyWithString() { FKeyField = newItem.ID, ID = Guid.NewGuid().ToString() });

            var result = from i in dStore.Query<TestItemForeignKeyWithString>()
                         join x in dStore.Query<TestItemPrimaryKey>() on i.FKeyField equals x.ID
                         select new
                         {
                             i,
                             x
                         };

            result.ToList().ForEach(R => Assert.IsTrue(!string.IsNullOrEmpty(R.x.Name)));
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_First()
        {
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });

            TestItemPrimaryKey item = dStore.Query<TestItemPrimaryKey>().First();
            Assert.IsNotNull(item);

            item = dStore.Query<TestItemPrimaryKey>().FirstOrDefault();
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Skip_Take()
        {
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });

            TestItemPrimaryKey item = dStore.Query<TestItemPrimaryKey>().OrderBy(R => R.ID).Skip(2).Take(1).First();
            Assert.IsNotNull(item);
        }


        [TestMethod]
        public virtual async Task Test_Can_Do_Order_By()
        {
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Hello", ID = Guid.NewGuid().ToString() });

            TestItemPrimaryKey item = dStore.Query<TestItemPrimaryKey>().OrderBy(R => R.ID).First();
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public virtual async Task Test_can_do_max()
        {
            await dStore.InsertObject(new TestItem());
            await dStore.InsertObject(new TestItem());
            await dStore.InsertObject(new TestItem());
            await dStore.InsertObject(new TestItem());

            int ti = dStore.Query<TestItem>().Max(r => r.id);
            Assert.IsTrue(ti == 4);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Starts_With()
        {
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Puppy ", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Puffin", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Porcupine", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Peacock ", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Platypus", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Doggy", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Kitten", ID = Guid.NewGuid().ToString() });

            var items = dStore.Query<TestItemPrimaryKey>().Where(R => R.Name.StartsWith("P"));
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 5);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Ends_With()
        {
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Puppy", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Puffin", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Porcupine", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Peacock ", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Platypus", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Doggy", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Kitten", ID = Guid.NewGuid().ToString() });

            var items = dStore.Query<TestItemPrimaryKey>().Where(R => R.Name.EndsWith("y"));
            Assert.IsTrue(items != null);
            Assert.IsTrue(items.Count() == 2);
        }

        [TestMethod]
        public virtual async Task Test_Can_Delete_With_Expression()
        {
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Puppy", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Puffin", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Porcupine", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Peacock ", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Platypus", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Doggy", ID = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemPrimaryKey() { Name = "Kitten", ID = Guid.NewGuid().ToString() });

            int items = await dStore.DeleteObjects<TestItemPrimaryKey>(r => r.Name == "Doggy");
            Assert.IsTrue(items == 1);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Join_When_Two_Objects_Have_Same_Field()
        {
            TestJoinObjectSameFields o1 = new TestJoinObjectSameFields();
            o1.SortOrder = 10;
            await dStore.InsertObject(o1);

            TestObjectSameFields o2 = new TestObjectSameFields();
            o2.SortOrder = 5;
            o2.ForeignKey = o1.ID;
            await dStore.InsertObject(o2);

            var data = from i in dStore.Query<TestObjectSameFields>()
                       join x in dStore.Query<TestJoinObjectSameFields>() on i.ForeignKey equals x.ID
                       select new
                       {
                           i.ID,
                           x.SortOrder,
                           SortOrder1 = i.SortOrder
                       };

            Assert.IsTrue(data.Count() == 1);
            Assert.IsTrue(data.First().SortOrder == 10);
            Assert.IsTrue(data.First().SortOrder1 == 5);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Projection_With_Different_Field_Names()
        {
            TestJoinObjectSameFields o1 = new TestJoinObjectSameFields();
            o1.SortOrder = 10;
            await dStore.InsertObject(o1);

            TestObjectSameFields o2 = new TestObjectSameFields();
            o2.SortOrder = 5;
            o2.ForeignKey = o1.ID;
            await dStore.InsertObject(o2);

            var data = from i in dStore.Query<TestObjectSameFields>()
                       join x in dStore.Query<TestJoinObjectSameFields>() on i.ForeignKey equals x.ID
                       select new
                       {
                           i.ID,
                           MySort = x.SortOrder,
                           SortOrder1 = i.SortOrder
                       };

            Assert.IsTrue(data.Count() == 1);
            Assert.IsTrue(data.First().MySort == 10);
            Assert.IsTrue(data.First().SortOrder1 == 5);
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Join_When_Two_Objects_Have_Same_Field_And_Only_Result_Field_Is_Aliased()
        {
            TestJoinObjectSameFields o1 = new TestJoinObjectSameFields();
            o1.SortOrder = 10;
            await dStore.InsertObject(o1);

            TestObjectSameFields o2 = new TestObjectSameFields();
            o2.SortOrder = 5;
            o2.ForeignKey = o1.ID;
            await dStore.InsertObject(o2);

            var data = from i in dStore.Query<TestObjectSameFields>()
                       join x in dStore.Query<TestJoinObjectSameFields>() on i.ForeignKey equals x.ID
                       select new
                       {
                           i.ID,
                           SortOrder1 = i.SortOrder
                       };

            Assert.IsTrue(data.Count() == 1);
            Assert.IsTrue(data.First().SortOrder1 == 5);
        }

        [TestMethod]
        public virtual async Task Test_Query_Method_Honors_Predicates()
        {
            await dStore.InsertObject(new TestItemWithPredicate() { Name = "foo", IsDeleted = false });
            await dStore.InsertObject(new TestItemWithPredicate() { Name = "bar", IsDeleted = true });

            Assert.IsTrue(dStore.Query<TestItemWithPredicate>().ToList().Count() == 1);
            Assert.IsTrue(dStore.Query<TestItemWithPredicate>().First().Name.Equals("foo"));
        }


        [TestMethod]
        public virtual async Task Test_Linq_Maps_Ids_Right_When_Projecting_More_Then_One_Object()
        {
            await dStore.InsertObject(new TestItem() { Something = "Hello" });
            await dStore.InsertObject(new TestItem() { Something = "Something2" });

            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 1 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 1 });

            var data = from i in dStore.Query<TestItem>()
                       join x in dStore.Query<TestItemWithForeignKey>() on i.id equals x.ForeignKey
                       where i.id == 1
                       select new { i, x };

            foreach (var v in data)
            {
                Assert.IsTrue(v != null);
                Assert.IsTrue(v.i.id == 1);
                Assert.IsTrue(v.x.id != 1);
            }
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_In()
        {
            await dStore.InsertObject(new TestItem() { Something = "Hello" });
            await dStore.InsertObject(new TestItem() { Something = "Something2" });
            await dStore.InsertObject(new TestItem() { Something = "Something Else" });

            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 3 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 3 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 1 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 2 });
            await dStore.InsertObject(new TestItemWithForeignKey() { Something = "Something2", ForeignKey = 1 });

            IEnumerable<int> ids = new List<int>() { 1, 2 };
            var items = dStore.Query<TestItemWithForeignKey>().Where(r => ids.Contains(r.ForeignKey)).ToList();
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Count == 6);
        }

        [TestMethod]
        public virtual async Task Test_Can_Map_Reuslt_Data_When_More_Than_One_Object_Requires_Same_Field_Name()
        {
            TestJoinObjectSameFields o1 = new TestJoinObjectSameFields();
            o1.SortOrder = 10;
            await dStore.InsertObject(o1);

            TestObjectSameFields o2 = new TestObjectSameFields();
            o2.SortOrder = 5;
            o2.ForeignKey = o1.ID;
            await dStore.InsertObject(o2);

            var data = from i in dStore.Query<TestObjectSameFields>()
                       join x in dStore.Query<TestJoinObjectSameFields>() on i.ForeignKey equals x.ID
                       select new
                       {
                           i,
                           x
                       };
        }

        [TestMethod]
        public virtual async Task Test_Can_Do_Group_By()
        {
            await dStore.InsertObject(new TestItemThreeFields() { something = "foo", something2 = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemThreeFields() { something = "foo", something2 = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemThreeFields() { something = "foo", something2 = Guid.NewGuid().ToString() });

            await dStore.InsertObject(new TestItemThreeFields() { something = "bar", something2 = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemThreeFields() { something = "bar", something2 = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemThreeFields() { something = "bar", something2 = Guid.NewGuid().ToString() });
            await dStore.InsertObject(new TestItemThreeFields() { something = "bar", something2 = Guid.NewGuid().ToString() });

            var result = dStore.Query<TestItemThreeFields>().GroupBy(r => r.something).Select(r => new { Count = r.Count(), r.Key }).ToList();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);
        }

        [TestMethod]
        public virtual async Task Test_Multiple_Objects_With_Same_Field_Projected_Map_Right_To_Anonymous_Object()
        {
            for (var i = 0; i < 10; i++)
                await dStore.InsertObject(new TestItem() { Something = "Test Item" });

            for (var i = 0; i < 10; i++)
                await dStore.InsertObject(new TestItemIgnoredField() { Something = "Test Item Ignored Field" });

            for (var i = 0; i < 10; i++)
                await dStore.InsertObject(new TestItemAdditionalInit() { Something = "Test Item Additional Init" });

            var query = from i in dStore.Query<TestItem>()
                        join x in dStore.Query<TestItemIgnoredField>() on i.id equals x.id
                        join z in dStore.Query<TestItemAdditionalInit>() on x.id equals z.id
                        where x.id > 5
                        select new
                        {
                            i.id,
                            TestItemIgnoredField = x.Something,
                            TestItem = i.Something,
                            TestItemAdditionalInit = z.Something
                        };

            var data = query.ToList();
            Assert.IsTrue(data.Count() == 5);
            foreach (var item in data)
            {
                Assert.IsTrue(item.id > 5);
                Assert.IsTrue(item.TestItem.Equals("Test Item"));
                Assert.IsTrue(item.TestItemIgnoredField.Equals("Test Item Ignored Field"));
                Assert.IsTrue(item.TestItemAdditionalInit.Equals("Test Item Additional Init"));
            }
        }
    }
}
