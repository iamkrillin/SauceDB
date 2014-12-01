using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public class Advantage : IDataBaseType
    {
        Controls.IDatabaseConnectionControl _control = new Controls.Advantage();

        public string Name { get { return "Advantage"; } }
        public Controls.IDatabaseConnectionControl ConnectionControl { get { return _control; } } 
    }
}
