using DataAccess.Core.Linq.Enums;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class BetweenExpression : DbExpression
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Gets the lower.
        /// </summary>
        public Expression Lower { get; private set; }

        /// <summary>
        /// Gets the upper.
        /// </summary>
        public Expression Upper { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetweenExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        public BetweenExpression(Expression expression, Expression lower, Expression upper)
            : base(DbExpressionType.Between, expression.Type)
        {
            this.Expression = expression;
            this.Lower = lower;
            this.Upper = upper;
        }
    }
}
