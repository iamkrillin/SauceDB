using System;

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
