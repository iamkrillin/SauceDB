using DataAccess.Core.Schema;
using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PerformanceHarness.Items
{
    public class SauceHarness : IPerformanceHarness
    {
        private DataAccess.Core.Interfaces.IDataStore dstore;
        public SauceHarness()
        {
            dstore = SqlServerConnection.GetDataStore("localhost", "sauce", "AppLogin", "AppLogin");
        }

        public void InsertTestObject(TestClass item)
        {
            dstore.InsertObject(item);
        }

        public void CleanUp()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("truncate table TestClass;");
                dstore.ExecuteCommand(cmd);
            }
            catch
            {

            }
        }

        public string Name
        {
            get
            {
                return "Sauce";
            }
        }

        public TestClass ReadObject(int p)
        {
            return dstore.LoadObject<TestClass>(p);
        }
    }
}
