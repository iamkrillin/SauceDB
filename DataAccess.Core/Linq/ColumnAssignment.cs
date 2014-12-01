using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Expressions;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// Linq stuff
    /// </summary>
    public class ColumnAssignment
    {
        /// <summary>
        /// Gets the column.
        /// </summary>
        public ColumnExpression Column { get; private set; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAssignment"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="expression">The expression.</param>
        public ColumnAssignment(ColumnExpression column, Expression expression)
        {
            this.Column = column;
            this.Expression = expression;
        }
    }
}
