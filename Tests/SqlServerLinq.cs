using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqlServerLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlServerConnection.GetDataStore(ConnectionStrings.SQL_SERVER);
        }
    }
}
