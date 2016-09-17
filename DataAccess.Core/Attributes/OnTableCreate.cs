using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core
{
    /// <summary>
    /// Indicates a function should be called when a table is created (Can optimally accept an IDataStore), must be static
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class OnTableCreateAttribute : Attribute
    {
    }
}
