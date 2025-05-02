using DataAccess.Core.Linq.Enums;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// A pairing of an expression and an order type for use in a SQL Order By clause
    /// </summary>
    public class OrderExpression
    {
        /// <summary>
        /// Gets the type of the order.
        /// </summary>
        /// <value>
        /// The type of the order.
        /// </value>
        public OrderType OrderType { get; private set; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderExpression"/> class.
        /// </summary>
        /// <param name="orderType">Type of the order.</param>
        /// <param name="expression">The expression.</param>
        public OrderExpression(OrderType orderType, Expression expression)
        {
            this.OrderType = orderType;
            this.Expression = expression;
        }
    }
}
