using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class RedundantSubqueryGatherer : DbExpressionVisitor
    {
        List<SelectExpression> redundant;

        /// <summary>
        /// Prevents a default instance of the <see cref="RedundantSubqueryGatherer"/> class from being created.
        /// </summary>
        private RedundantSubqueryGatherer()
        {
        }

        internal static List<SelectExpression> Gather(Expression source)
        {
            RedundantSubqueryGatherer gatherer = new RedundantSubqueryGatherer();
            gatherer.Visit(source);
            return gatherer.redundant;
        }

        private static bool IsRedudantSubquery(SelectExpression select)
        {
            return (RedundantSubqueryRemover.IsSimpleProjection(select) || RedundantSubqueryRemover.IsNameMapProjection(select))
                    && !select.IsDistinct && !select.IsReverse && select.Take == null && select.Skip == null && select.Where == null &&
                    (select.OrderBy == null || select.OrderBy.Count == 0) && (select.GroupBy == null || select.GroupBy.Count == 0);
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            if (IsRedudantSubquery(select))
            {
                if (this.redundant == null)
                {
                    this.redundant = new List<SelectExpression>();
                }
                this.redundant.Add(select);
            }
            return select;
        }

        /// <summary>
        /// Visits the subquery.
        /// </summary>
        /// <param name="subquery">The subquery.</param>
        /// <returns></returns>
        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            // don't gather inside scalar & exists
            return subquery;
        }
    }
}
