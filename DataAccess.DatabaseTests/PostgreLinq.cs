using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DataAccess.PostgreSQL;
using DataAccess.Core.Interfaces;
using DataAccess.DatabaseTests.Tests;
using DataAccess.Core.Data;
using System.Data;

namespace DataAccess.DatabaseTests
{
    public class PostgreLinq : LinqTests
    {
        public override IDataStore GetDataStore()
        {
            return PostgreSQLServerConnection.GetDataStore("Server=postgre;Port=5432;Database=Sauce;User Id=sauce;Password=Sauce;");
        }

        public PostgreLinq()
            : base()
        {
            dStore = GetDataStore();
            InitHelper.InitDataStore(dStore);
        }
    }
}
