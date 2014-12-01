using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataAccess.Extensions
{
    public static class DatabaseMigrator
    {

        /// <summary>
        /// Runs all the types in the assembly namespace combination and run them through the type validator on the datastore
        /// If your schema validator will modifies the schema, this will effectively migrate it       
        /// </summary>
        /// <param name="dstore">The datastore</param>
        /// <param name="asmb"></param>
        /// <param name="NameSpace"></param>
        public static void MigrateDatastore(this IDataStore dstore, Assembly asmb, string NameSpace)
        {
            List<Type> data = asmb.GetTypes().Where(r => r.Namespace.Equals(NameSpace)).ToList();
            foreach (Type t in data)
            {
                dstore.TypeInformationParser.GetTypeInfo(t);
            }
        }
    }
}
