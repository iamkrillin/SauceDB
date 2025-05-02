using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnGatherer : DbExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, ColumnExpression> columns = new Dictionary<string, ColumnExpression>();

        /// <summary>
        /// Gathers the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static IEnumerable<ColumnExpression> Gather(Expression expression)
        {
            var gatherer = new ColumnGatherer();
            gatherer.Visit(expression);
            return gatherer.columns.Values;
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (!this.columns.ContainsKey(column.Name))
            {
                this.columns.Add(column.Name, column);
            }
            return column;
        }
    }
}
