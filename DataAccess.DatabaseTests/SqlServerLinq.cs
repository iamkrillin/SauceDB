using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.DatabaseTests
{
    [TestClass]
    public class SqlServerLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore("Data Source=sql;Initial Catalog=Sauce;User Id=AppLogin;Password=AppLogin;");
        }
    }
}
