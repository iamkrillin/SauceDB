using System;

namespace DataAccess.Core.Attributes
{
    /// <summary>
    /// Indicates that the type should not be validated against the db schema, useful for views and stored procedures
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BypassValidationAttribute : Attribute
    {
    }
}
