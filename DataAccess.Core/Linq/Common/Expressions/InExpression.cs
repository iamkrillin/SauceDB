using DataAccess.Core.Linq.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class InExpression : SubqueryExpression
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public ReadOnlyCollection<Expression> Values { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="select">The select.</param>
        public InExpression(Expression expression, SelectExpression select)
            : base(DbExpressionType.In, typeof(bool), select)
        {
            this.Expression = expression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="values">The values.</param>
        public InExpression(Expression expression, IEnumerable<Expression> values)
            : base(DbExpressionType.In, typeof(bool), null)
        {
            this.Expression = expression;
            this.Values = values.ToReadOnly();
        }
    }
}
