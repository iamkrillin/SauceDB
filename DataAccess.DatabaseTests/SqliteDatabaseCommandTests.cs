using DataAccess.Core.Interfaces;
using DataAccess.DatabaseTests.Tests;
using DataAccess.SqlCompact;
using DataAccess.SQLite;
using DataAccess.SqlServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace DataAccess.DatabaseTests
{
    public class SqliteDatabaseCommandTests : DatabaseCommandTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }
    }
}
