using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public static class DatabaseManager
    {
        public static List<IDataBaseType> Databases { get; private set; }

        static DatabaseManager()
        {
            Databases = new List<IDataBaseType>();
            Databases.Add(new SqlServerPasswordSecurity());
            Databases.Add(new SqlServerIntegratedSecurity());
            Databases.Add(new MySQL());
            Databases.Add(new Postgre());
            Databases.Add(new SQLite());
            Databases.Add(new Advantage());
        }
    }
}
