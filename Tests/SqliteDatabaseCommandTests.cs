using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using Tests.Tests;

namespace Tests
{
    [TestClass]
    public class SqliteDatabaseCommandTests : DatabaseCommandTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }
    }
}
