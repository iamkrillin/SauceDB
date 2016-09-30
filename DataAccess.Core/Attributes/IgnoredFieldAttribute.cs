using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Attributes
{
    /// <summary>
    /// Indicates that a field should be ignored, useful for calculated fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoredFieldAttribute : Attribute
    {
    }
}
