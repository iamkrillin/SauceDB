using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using System.Configuration;
using DataAccess.MySql;
using DataAccess.PostgreSQL;
using DataAccess.SQLite;
using System.Data;

namespace DataAccess.Samples
{
    class Program
    {
        static void Main()
        {
            SampleRunner.RunMe();
        }
    }
}
