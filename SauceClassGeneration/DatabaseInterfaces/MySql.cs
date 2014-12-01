using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.MySql;
using TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public class MySQL : IDataBaseType
    {
        Controls.IDatabaseConnectionControl _control = new Controls.MySql();

        public string Name { get { return "MySQL"; } }
        public IDatabaseConnectionControl ConnectionControl { get { return _control; } }
    }
}
