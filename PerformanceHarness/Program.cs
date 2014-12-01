using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using PerformanceHarness.Items;

namespace PerformanceHarness
{
    class Program
    {
        private static List<IPerformanceHarness> _items;
        static Program()
        {
            _items = new List<IPerformanceHarness>();
            _items.Add(new SauceHarness());
            _items.Add(new ADOHarness());
        }


        static void Main(string[] args)
        {
            DoInsertTiming();
            DoReadTiming();


            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void DoReadTiming()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Starting Read Test!");
            TestClass tc = new TestClass();
            tc.Name = Guid.NewGuid().ToString();

            foreach (var v in _items)
            {
                v.CleanUp();
                Stopwatch sw = new Stopwatch();
                sw.Start();

                v.InsertTestObject(tc);

                for (int i = 0; i < 10000; i++)
                    v.ReadObject(tc.ID);

                sw.Stop();
                Console.WriteLine("{0} Done in {1}ms {2}avg!", v.Name, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / 10000.0);
                v.CleanUp();
            }
        }

        private static void DoInsertTiming()
        {
            Console.WriteLine("Starting Insert Test!");
            TestClass tc = new TestClass();
            //create the table
            _items[0].InsertTestObject(new TestClass() { Name = "foo" });


            tc.Name = Guid.NewGuid().ToString();
            
            foreach (var v in _items)
            {
                v.CleanUp();
                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < 10000; i++)
                    v.InsertTestObject(tc);

                sw.Stop();
                Console.WriteLine("{0} Done in {1}ms {2}avg!", v.Name, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds / 10000.0);
                v.CleanUp();
            }
        }
    }
}
