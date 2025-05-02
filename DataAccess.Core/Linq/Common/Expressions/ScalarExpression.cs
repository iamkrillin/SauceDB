using DataAccess.Core.Linq.Enums;
using System;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class ScalarExpression : SubqueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarExpression"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="select">The select.</param>
        public ScalarExpression(Type type, SelectExpression select)
            : base(DbExpressionType.Scalar, type, select)
        {
        }
    }
}
