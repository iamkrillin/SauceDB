using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.PostgreSQL;
using TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public class Postgre : IDataBaseType
    {
        Controls.IDatabaseConnectionControl _control = new Controls.Postgre();

        public string Name { get { return "Postgre"; } }
        public IDatabaseConnectionControl ConnectionControl { get { return _control; } }
    }
}
