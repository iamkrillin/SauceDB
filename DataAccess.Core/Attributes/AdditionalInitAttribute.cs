using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Attributes
{
    /// <summary>
    /// Indicates that the function should be called when an object is loaded (Can optionally accept an IDataStore)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AdditionalInitAttribute : Attribute
    {
    }
}
