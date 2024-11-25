using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using DataAccess.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
