using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// Allows is-null tests against value-types like int and float
    /// </summary>
    public class IsNullExpression : DbExpression
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullExpression"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public IsNullExpression(Expression expression)
            : base(DbExpressionType.IsNull, typeof(bool))
        {
            this.Expression = expression;
        }
    }
}
