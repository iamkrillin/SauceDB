using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    internal class AggregateGatherer : DbExpressionVisitor
    {
        List<AggregateSubqueryExpression> aggregates = new List<AggregateSubqueryExpression>();
        private AggregateGatherer()
        {
        }

        internal static List<AggregateSubqueryExpression> Gather(Expression expression)
        {
            AggregateGatherer gatherer = new AggregateGatherer();
            gatherer.Visit(expression);
            return gatherer.aggregates;
        }

        /// <summary>
        /// Visits the aggregate subquery.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        protected override Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            this.aggregates.Add(aggregate);
            return base.VisitAggregateSubquery(aggregate);
        }
    }
}
