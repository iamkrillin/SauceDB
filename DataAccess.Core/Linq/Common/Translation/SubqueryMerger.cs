using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class SubqueryMerger : DbExpressionVisitor
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="SubqueryMerger"/> class from being created.
        /// </summary>
        private SubqueryMerger()
        {
        }

        /// <summary>
        /// Merges the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static Expression Merge(Expression expression)
        {
            return new SubqueryMerger().Visit(expression);
        }

        bool isTopLevel = true;

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            bool wasTopLevel = isTopLevel;
            isTopLevel = false;

            select = (SelectExpression)base.VisitSelect(select);

            // next attempt to merge subqueries that would have been removed by the above
            // logic except for the existence of a where clause
            while (CanMergeWithFrom(select, wasTopLevel))
            {
                SelectExpression fromSelect = GetLeftMostSelect(select.From);

                // remove the redundant subquery
                select = SubqueryRemover.Remove(select, fromSelect);

                // merge where expressions 
                Expression where = select.Where;
                if (fromSelect.Where != null)
                {
                    if (where != null)
                    {
                        where = fromSelect.Where.And(where);
                    }
                    else
                    {
                        where = fromSelect.Where;
                    }
                }
                var orderBy = select.OrderBy != null && select.OrderBy.Count > 0 ? select.OrderBy : fromSelect.OrderBy;
                var groupBy = select.GroupBy != null && select.GroupBy.Count > 0 ? select.GroupBy : fromSelect.GroupBy;
                Expression skip = select.Skip != null ? select.Skip : fromSelect.Skip;
                Expression take = select.Take != null ? select.Take : fromSelect.Take;
                bool isDistinct = select.IsDistinct | fromSelect.IsDistinct;

                if (where != select.Where || orderBy != select.OrderBy || groupBy != select.GroupBy || isDistinct != select.IsDistinct || skip != select.Skip || take != select.Take)
                {
                    select = new SelectExpression(select.Alias, select.Columns, select.From, where, orderBy, groupBy, isDistinct, skip, take, select.IsReverse);
                }
            }

            return select;
        }

        private static SelectExpression GetLeftMostSelect(Expression source)
        {
            SelectExpression select = source as SelectExpression;
            if (select != null) return select;
            JoinExpression join = source as JoinExpression;
            if (join != null) return GetLeftMostSelect(join.Left);
            return null;
        }

        private static bool IsColumnProjection(SelectExpression select)
        {
            for (int i = 0, n = select.Columns.Count; i < n; i++)
            {
                var cd = select.Columns[i];
                if (cd.Expression.NodeType != (ExpressionType)DbExpressionType.Column && cd.Expression.NodeType != ExpressionType.Constant)
                    return false;
            }
            return true;
        }

        private static bool CanMergeWithFrom(SelectExpression select, bool isTopLevel)
        {
            SelectExpression fromSelect = GetLeftMostSelect(select.From);
            if (fromSelect == null)
                return false;
            if (!IsColumnProjection(fromSelect))
                return false;
            bool selHasNameMapProjection = RedundantSubqueryRemover.IsNameMapProjection(select);
            bool selHasOrderBy = select.OrderBy != null && select.OrderBy.Count > 0;
            bool selHasGroupBy = select.GroupBy != null && select.GroupBy.Count > 0;
            bool selHasAggregates = AggregateChecker.HasAggregates(select);
            bool selHasJoin = select.From is JoinExpression;
            bool frmHasOrderBy = fromSelect.OrderBy != null && fromSelect.OrderBy.Count > 0;
            bool frmHasGroupBy = fromSelect.GroupBy != null && fromSelect.GroupBy.Count > 0;
            bool frmHasAggregates = AggregateChecker.HasAggregates(fromSelect);
            // both cannot have orderby
            if (selHasOrderBy && frmHasOrderBy)
                return false;
            // both cannot have groupby
            if (selHasGroupBy && frmHasGroupBy)
                return false;
            // these are distinct operations 
            if (select.IsReverse || fromSelect.IsReverse)
                return false;
            // cannot move forward order-by if outer has group-by
            if (frmHasOrderBy && (selHasGroupBy || selHasAggregates || select.IsDistinct))
                return false;
            // cannot move forward group-by if outer has where clause
            if (frmHasGroupBy /*&& (select.Where != null)*/) // need to assert projection is the same in order to move group-by forward
                return false;
            // cannot move forward a take if outer has take or skip or distinct
            if (fromSelect.Take != null && (select.Take != null || select.Skip != null || select.IsDistinct || selHasAggregates || selHasGroupBy || selHasJoin))
                return false;
            // cannot move forward a skip if outer has skip or distinct
            if (fromSelect.Skip != null && (select.Skip != null || select.IsDistinct || selHasAggregates || selHasGroupBy || selHasJoin))
                return false;
            // cannot move forward a distinct if outer has take, skip, groupby or a different projection
            if (fromSelect.IsDistinct && (select.Take != null || select.Skip != null || !selHasNameMapProjection || selHasGroupBy || selHasAggregates || (selHasOrderBy && !isTopLevel) || selHasJoin))
                return false;
            if (frmHasAggregates && (select.Take != null || select.Skip != null || select.IsDistinct || selHasAggregates || selHasGroupBy || selHasJoin))
                return false;
            return true;
        }
    }
}
