using DataAccess.Core.Data;
using DataAccess.Core.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DataAccess.Migrations
{
    class Program
    {
        private static string _path;

        static void Main(string[] args)
        {
            Arguments argdata = new Arguments(args);
            _path = Path.GetDirectoryName(argdata.DllPath);
            Console.WriteLine("Using Dll Path: {0}", _path);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Assembly asmb = Assembly.LoadFile(argdata.DllPath);

            var migrators = asmb.GetTypes().Where(r => r.IsSubclassOf(typeof(DBMigrator)));
            foreach (var v in migrators)
            {
                DBMigrator mig = (DBMigrator)v.GetConstructor(Type.EmptyTypes).Invoke(null);
                mig.ExceptionOnError = argdata.ExceptionOnStepError;
                Console.WriteLine("Processing {0}", mig.ToString());
                mig.RunMigrations(asmb);

#if(DEBUG)
              Console.ReadLine();
#endif
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Split(',').First().Trim() + ".dll";
            if (File.Exists(_path + name))
            {
                return Assembly.LoadFile(_path + name);
            }
            else
            {
                return null;
            }
        }
    }
}
