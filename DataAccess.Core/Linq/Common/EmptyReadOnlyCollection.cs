using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class EmptyReadOnlyCollection<T>
    {
        /// <summary>
        /// 
        /// </summary>
        internal static readonly ReadOnlyCollection<T> Empty = new List<T>().AsReadOnly();
    }
}
