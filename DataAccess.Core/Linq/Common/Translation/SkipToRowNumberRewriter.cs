// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Rewrites take and skip expressions into uses of TSQL row_number function
    /// </summary>
    public class SkipToRowNumberRewriter : DbExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        QueryLanguage language;

        /// <summary>
        /// Prevents a default instance of the <see cref="SkipToRowNumberRewriter"/> class from being created.
        /// </summary>
        /// <param name="language">The language.</param>
        private SkipToRowNumberRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        /// <summary>
        /// Rewrites the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new SkipToRowNumberRewriter(language).Visit(expression);
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression)base.VisitSelect(select);
            if (select.Skip != null)
            {
                SelectExpression newSelect = select.SetSkip(null).SetTake(null);
                bool canAddColumn = !select.IsDistinct && (select.GroupBy == null || select.GroupBy.Count == 0);
                if (!canAddColumn)
                {
                    newSelect = newSelect.AddRedundantSelect(this.language, new TableAlias());
                }
                newSelect = newSelect.AddColumn(new ColumnDeclaration("_rownum", new RowNumberExpression(select.OrderBy)));

                // add layer for WHERE clause that references new rownum column
                newSelect = newSelect.AddRedundantSelect(this.language, new TableAlias());
                newSelect = newSelect.RemoveColumn(newSelect.Columns.Single(c => c.Name.Equals("_rownum", StringComparison.InvariantCultureIgnoreCase)));

                var newAlias = ((SelectExpression)newSelect.From).Alias;
                ColumnExpression rnCol = new ColumnExpression(typeof(int), newAlias, "_rownum");
                Expression where;
                if (select.Take != null)
                {
                    where = new BetweenExpression(rnCol, Expression.Add(select.Skip, Expression.Constant(1)), Expression.Add(select.Skip, select.Take));
                }
                else
                {
                    where = rnCol.GreaterThan(select.Skip);
                }
                if (newSelect.Where != null)
                {
                    where = newSelect.Where.And(where);
                }
                newSelect = newSelect.SetWhere(where);

                select = newSelect;
            }
            return select;
        }
    }
}