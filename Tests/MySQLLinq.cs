using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class MySQLLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return MySqlServerConnection.GetDataStore(ConnectionStrings.MYSQL);
        }
    }
}
