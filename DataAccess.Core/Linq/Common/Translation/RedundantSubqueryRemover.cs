// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Removes select expressions that don't add any additional semantic value
    /// </summary>
    public class RedundantSubqueryRemover : DbExpressionVisitor
    {
        private RedundantSubqueryRemover()
        {
        }

        public static Expression Remove(Expression expression)
        {
            expression = new RedundantSubqueryRemover().Visit(expression);
            expression = SubqueryMerger.Merge(expression);
            return expression;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression)base.VisitSelect(select);

            // first remove all purely redundant subqueries
            List<SelectExpression> redundant = RedundantSubqueryGatherer.Gather(select.From);
            if (redundant != null)
            {
                select = SubqueryRemover.Remove(select, redundant);
            }

            return select;
        }

        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            proj = (ProjectionExpression)base.VisitProjection(proj);
            if (proj.Select.From is SelectExpression)
            {
                List<SelectExpression> redundant = RedundantSubqueryGatherer.Gather(proj.Select);
                if (redundant != null)
                {
                    proj = SubqueryRemover.Remove(proj, redundant);
                }
            }
            return proj;
        }

        internal static bool IsSimpleProjection(SelectExpression select)
        {
            foreach (ColumnDeclaration decl in select.Columns)
            {
                ColumnExpression col = decl.Expression as ColumnExpression;
                if (col == null || decl.Name != col.Name)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsNameMapProjection(SelectExpression select)
        {
            if (select.From is TableExpression) return false;
            SelectExpression fromSelect = select.From as SelectExpression;
            if (fromSelect == null || select.Columns.Count != fromSelect.Columns.Count)
                return false;
            ReadOnlyCollection<ColumnDeclaration> fromColumns = fromSelect.Columns;
            // test that all columns in 'select' are referring to columns in the same position
            // in from.
            for (int i = 0, n = select.Columns.Count; i < n; i++)
            {
                ColumnExpression col = select.Columns[i].Expression as ColumnExpression;
                if (col == null || !(col.Name == fromColumns[i].Name))
                    return false;
            }
            return true;
        }

        internal static bool IsInitialProjection(SelectExpression select)
        {
            return select.From is TableExpression;
        }
    }
}