using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqLiteLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }
    }
}
