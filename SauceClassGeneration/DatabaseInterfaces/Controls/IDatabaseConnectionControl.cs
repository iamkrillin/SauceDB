using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls
{
    public interface IDatabaseConnectionControl
    {
        IDataStore GetConnection();
    }
}
