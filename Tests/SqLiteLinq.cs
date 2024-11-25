using DataAccess.Core.Interfaces;
using DataAccess.SQLite;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
