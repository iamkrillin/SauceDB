using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Attributes
{
    /// <summary>
    /// Indicates the object needs to a query predicate
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class QueryPredicateAttribute : Attribute
    {
    }
}
