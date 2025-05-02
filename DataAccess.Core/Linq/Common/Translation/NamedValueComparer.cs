using DataAccess.Core.Linq.Common.Expressions;
using System;
using System.Collections.Generic;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class NamedValueComparer : IEqualityComparer<NamedValueExpression>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(NamedValueExpression x, NamedValueExpression y)
        {
            return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
        public int GetHashCode(NamedValueExpression obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
