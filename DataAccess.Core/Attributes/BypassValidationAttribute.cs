using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core
{
    /// <summary>
    /// Indicates that the type should not be validated against the db schema, useful for views and stored procedures
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BypassValidationAttribute : Attribute
    {
    }
}
