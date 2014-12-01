using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheCodeHaven.SauceClassGeneration.DatabaseInterfaces.Controls;

namespace TheCodeHaven.SauceClassGeneration.DatabaseInterfaces
{
    public interface IDataBaseType
    {
        string Name { get; }
        IDatabaseConnectionControl ConnectionControl { get; }
    }
}
