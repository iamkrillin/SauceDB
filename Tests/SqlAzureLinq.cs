using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqlAzureLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore(ConnectionStrings.SQL_SERVER);
        }
    }
}
