using System;

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
