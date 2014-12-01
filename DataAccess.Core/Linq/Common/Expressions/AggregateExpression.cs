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
    public class AggregateExpression : DbExpression
    {
        /// <summary>
        /// Gets the name of the aggregate.
        /// </summary>
        /// <value>
        /// The name of the aggregate.
        /// </value>
        public string AggregateName { get; private set; }

        /// <summary>
        /// Gets the argument.
        /// </summary>
        public Expression Argument { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is distinct.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is distinct; otherwise, <c>false</c>.
        /// </value>
        public bool IsDistinct { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateExpression"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="aggregateName">Name of the aggregate.</param>
        /// <param name="argument">The argument.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        public AggregateExpression(Type type, string aggregateName, Expression argument, bool isDistinct)
            : base(DbExpressionType.Aggregate, type)
        {
            this.AggregateName = aggregateName;
            this.Argument = argument;
            this.IsDistinct = isDistinct;
        }
    }
}
