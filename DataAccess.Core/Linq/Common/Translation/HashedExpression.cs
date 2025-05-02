using DataAccess.Core.Linq.Common.Expressions;
using System;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public struct HashedExpression : IEquatable<HashedExpression>
    {
        Expression expression;
        int hashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedExpression"/> struct.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public HashedExpression(Expression expression)
        {
            this.expression = expression;
            this.hashCode = Hasher.ComputeHash(expression);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HashedExpression))
                return false;
            return this.Equals((HashedExpression)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(HashedExpression other)
        {
            return this.hashCode == other.hashCode && DbExpressionComparer.AreEqual(this.expression, other.expression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
