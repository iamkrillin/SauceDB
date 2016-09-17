using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core
{
    /// <summary>
    /// Decorate to indicate an object is loaded from a view
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewAttribute : TableNameAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ViewAttribute()
        {
        }
    }
}
