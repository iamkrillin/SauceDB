using DataAccess.Core.Interfaces;

namespace Tests.Tests
{
    [TestClass]
    public abstract class DatabaseCommandTests
    {
        protected IDataStore dStore;
        public abstract IDataStore GetDataStore();

        public DatabaseCommandTests()
        {
            dStore = GetDataStore();
            dStore.InitDataStore().Wait();
        }
    }
}
