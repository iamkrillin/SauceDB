using DataAccess.Core;
using DataAccess.Core.Data;
using DataAccess.DatabaseTests.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class TypeParserTests
    {
        private TypeParser _parser;

        public TypeParserTests()
        {
            _parser = new TypeParser(new FakeDataStore());
        }

        [TestMethod]
        public void Test_FieldType_IsParsed_User_String()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItemDifferentFieldType));
            Assert.IsTrue(ti.DataFields[0].DataFieldType == Core.Attributes.FieldType.UserString);
            Assert.IsTrue(ti.DataFields[0].DataFieldString == "varchar(1000)");
        }

        [TestMethod]
        public void Test_Field_Lenth_Is_Parsed()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItemSmallString));
            Assert.IsTrue(ti.DataFields[1].FieldLength == 5);
        }

        [TestMethod]
        public void Test_Field_Length_Is_Null_When_Not_Specified()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItem));
            foreach (var v in ti.DataFields)
                Assert.IsTrue(!v.FieldLength.HasValue);
        }

        [TestMethod]
        public void Test_FieldType_Is_Default_When_Not_Specified()
        {
            DatabaseTypeInfo ti = _parser.GetTypeInfo(typeof(TestItem));
            Assert.IsNotNull(ti);
            Assert.IsTrue(ti.DataFields.Count == 3);
            foreach (var v in ti.DataFields)
                Assert.IsTrue(v.DataFieldType == Core.Attributes.FieldType.Default);
        }

        [TestMethod]
        public void Test_Type_Parser_Is_Thread_Safe()
        {
            ConcurrentBag<DatabaseTypeInfo> infos = new ConcurrentBag<DatabaseTypeInfo>();
            List<Thread> threads = new List<Thread>();
            int numThreads = 50;
            int numGets = 500;

            for (var i = 0; i < numThreads; i++)
            {
                threads.Add(new Thread(() =>
                {
                    for (var x = 0; x < numGets; x++)
                    {
                        DatabaseTypeInfo dti = _parser.GetTypeInfo(typeof(TestItemThreeFields));

                        if (dti == null)
                            throw new Exception("Null Type Info");

                        infos.Add(dti);
                    }
                }));
            }

            threads.ForEach(r => r.Start());

            while (threads.Count(r => r.ThreadState == ThreadState.Running) > 0)
                Thread.Sleep(250);

            Assert.IsTrue(infos.Count() == numGets * numThreads);

            foreach (DatabaseTypeInfo info in infos)
            {
                foreach (DatabaseTypeInfo toCompare in infos)
                {
                    Assert.IsTrue(info == toCompare);
                }
            }
        }
    }
}
