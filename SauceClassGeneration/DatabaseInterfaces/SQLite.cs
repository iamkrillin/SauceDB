using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public class SQLite : IDataBaseType
    {
        Controls.IDatabaseConnectionControl _control = new Controls.SQLite();

        public string Name { get { return "SQLite"; } }
        public IDatabaseConnectionControl ConnectionControl { get { return _control; } }
    }
}
