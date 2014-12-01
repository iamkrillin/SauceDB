using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Expressions;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class Hasher : DbExpressionVisitor
    {
        int hc;

        /// <summary>
        /// Computes the hash.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static int ComputeHash(Expression expression)
        {
            var hasher = new Hasher();
            hasher.Visit(expression);
            return hasher.hc;
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            hc = hc + ((c.Value != null) ? c.Value.GetHashCode() : 0);
            return c;
        }
    }
}
