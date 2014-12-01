using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class OuterJoinedExpression : DbExpression
    {
        /// <summary>
        /// Gets the test.
        /// </summary>
        public Expression Test { get; private set; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OuterJoinedExpression"/> class.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="expression">The expression.</param>
        public OuterJoinedExpression(Expression test, Expression expression)
            : base(DbExpressionType.OuterJoined, expression.Type)
        {
            this.Test = test;
            this.Expression = expression;
        }
    }
}
