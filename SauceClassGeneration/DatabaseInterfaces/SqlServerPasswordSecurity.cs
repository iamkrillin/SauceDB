using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.SqlServer;
using TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public class SqlServerPasswordSecurity : IDataBaseType
    {
        Controls.IDatabaseConnectionControl _control = new Controls.SqlServerPasswordAuth();

        public string Name { get { return "SqlServer - Password Auth"; } }
        public IDatabaseConnectionControl ConnectionControl { get { return _control; } }
    }
}
