using DataAccess.Migrations;
using DataAccess.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Migrations
{
    public class Migrator : DBMigrator
    {
        protected override DataAccess.Core.Interfaces.IDataStore GetDataStore()
        {
            return new MySqlDataStore("localhost", "test", "AppLogin", "AppLogin");
        }
    }
}
