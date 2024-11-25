using DataAccess.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Tests.DataObjects;

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
