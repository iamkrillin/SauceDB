using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class PostgreLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore(ConnectionStrings.POSTGRE);
        }

        public PostgreLinq()
            : base()
        {
            dStore = GetDataStore();
            dStore.InitDataStore().Wait();
        }
    }
}
