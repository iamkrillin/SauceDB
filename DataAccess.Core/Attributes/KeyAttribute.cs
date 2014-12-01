using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Attributes
{
    /// <summary>
    /// Indicates that a field represents the primary key of an object
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : DataFieldAttribute
    {
        /// <summary>
        /// Default constructor, initializes SetOnInsert=false
        /// </summary>
        public KeyAttribute()
        {
            SetOnInsert = false;
        }
    }
}
