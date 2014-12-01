using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Enums;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// A custom expression node representing a SQL join clause
    /// </summary>
    public class JoinExpression : DbExpression
    {
        /// <summary>
        /// Gets the join.
        /// </summary>
        public JoinType Join { get; private set; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        public Expression Left { get; private set; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        public Expression Right { get; private set; }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        public new Expression Condition { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinExpression"/> class.
        /// </summary>
        /// <param name="joinType">Type of the join.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="condition">The condition.</param>
        public JoinExpression(JoinType joinType, Expression left, Expression right, Expression condition)
            : base(DbExpressionType.Join, typeof(void))
        {
            this.Join = joinType;
            this.Left = left;
            this.Right = right;
            this.Condition = condition;
        }
    }
}
