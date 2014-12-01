using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using DataAccess.Core.Data;
using System.Data.SQLite;
using DataAccess.Core;
using DataAccess.DatabaseTests.DataObjects;
using System.IO;

namespace DataAccess.DatabaseTests
{
    public class SqLiteLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return SqlLiteDataConnection.GetDataStore(Path.GetTempFileName());
        }
    }
}
