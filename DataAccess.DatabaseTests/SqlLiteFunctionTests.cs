using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using DataAccess.Core.Data;
using System.Data.SQLite;
using DataAccess.DatabaseTests.DataObjects;
using System.IO;
using DataAccess.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlLiteFunctionTests : FunctionTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }

        public SqlLiteFunctionTests()
        {
            dstore = SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
            InitHelper.AttachEvents(dstore);

            List<DBObject> data = dstore.SchemaValidator.ViewValidator.GetObjects().Where(R => R.Name != "sqlite_sequence").ToList();
            while (data.Count != 0)
            {
                foreach (DBObject t in data)
                {
                    if (t.Name.Equals("sqlite_sequence")) continue;
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.CommandText = string.Format("DROP VIEW {0}", t.Name);
                    try { dstore.ExecuteCommand(cmd); }
                    catch { }
                }
                data = dstore.SchemaValidator.ViewValidator.GetObjects(true).Where(R => R.Name != "sqlite_sequence").ToList();
            }

            data = dstore.SchemaValidator.TableValidator.GetObjects().Where(R => R.Name != "sqlite_sequence").ToList();
            while (data.Count != 0)
            {
                foreach (DBObject t in data)
                {
                    if (t.Name.Equals("sqlite_sequence")) continue;
                    SQLiteCommand cmd = new SQLiteCommand();
                    cmd.CommandText = string.Format("DROP TABLE {0}", t.Name);
                    try { dstore.ExecuteCommand(cmd); }
                    catch { }
                }
                data = dstore.SchemaValidator.TableValidator.GetObjects(true).Where(R => R.Name != "sqlite_sequence").ToList();
            }
        }

        public override void Test_Can_Get_Escape_Sequences()
        {
            Assert.IsTrue(dstore.Connection.LeftEscapeCharacter.Equals("\""));
            Assert.IsTrue(dstore.Connection.RightEscapeCharacter.Equals("\""));
        }

#if(!DEBUG)
        public override void Test_Bulk_Insert()
        {
            List<TestItem> items = new List<TestItem>();
            for (int i = 0; i < 100; i++)
            {
                items.Add(new TestItem()
                {
                    id = i,
                    Something = "something",
                    TimeSpent = new TimeSpan(100)
                });
            }

            dstore.InsertObjects(items);

            List<TestItem> loaded = dstore.LoadEntireTable<TestItem>().ToList();
            Assert.IsTrue(loaded.Count == 100);
        }
#endif

        public class Foo
        {
            public int ID { get; set; }
            public bool Default { get; set; }
        }

        [View(TableName = "foobar")]
        public class FooBar
        {
            public int ID { get; set; }
            public bool Default { get; set; }
        }

        [TestMethod]
        public void Test_Can_Load_With_Keyword()
        {
            dstore.InsertObject(new Foo() { Default = true });
            Assert.IsTrue(dstore.LoadObject<Foo>(1).Default);

            dstore.GetCommand<int>().ExecuteCommand("create view foobar as select * from foos;");
            Assert.IsTrue(dstore.LoadObject<FooBar>(1).Default);
        }

        [TestMethod]
        public void Test_Can_Insert_With_Multiple_Threads()
        {
            List<Thread> _threads = new List<Thread>();

            for (int i = 0; i < 100; i++)
            {
                Thread thread = new Thread(() =>
                {
                    for (int x = 0; x < 1000; x++)
                    {
                        dstore.InsertObject(new Foo() { Default = true });
                    }
                });

                _threads.Add(thread);
                thread.Start();
            }

            while (_threads.Count() > 0)
            {
                Thread thrd = _threads[0];

                if (!thrd.IsAlive)
                    _threads.Remove(thrd);

                Thread.Sleep(500);
            }
        }
    }
}
