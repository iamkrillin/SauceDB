// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Rewrites queries with skip and take to use the nested queries with inverted ordering technique
    /// </summary>
    public class SkipToNestedOrderByRewriter : DbExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        QueryLanguage language;

        /// <summary>
        /// Prevents a default instance of the <see cref="SkipToNestedOrderByRewriter"/> class from being created.
        /// </summary>
        /// <param name="language">The language.</param>
        private SkipToNestedOrderByRewriter(QueryLanguage language)
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
            return new SkipToNestedOrderByRewriter(language).Visit(expression);
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression)base.VisitSelect(select);

            if (select.Skip != null && select.Take != null && select.OrderBy.Count > 0)
            {
                var skip = select.Skip;
                var take = select.Take;
                var skipPlusTake = PartialEvaluator.Eval(Expression.Add(skip, take));

                select = select.SetTake(skipPlusTake).SetSkip(null);
                select = select.AddRedundantSelect(this.language, new TableAlias());
                select = select.SetTake(take);

                // propagate order-bys to new layer
                select = (SelectExpression)OrderByRewriter.Rewrite(this.language, select);
                var inverted = select.OrderBy.Select(ob => new OrderExpression(ob.OrderType == OrderType.Ascending ? OrderType.Descending : OrderType.Ascending, ob.Expression));
                select = select.SetOrderBy(inverted);

                select = select.AddRedundantSelect(this.language, new TableAlias());
                select = select.SetTake(Expression.Constant(0)); // temporary
                select = (SelectExpression)OrderByRewriter.Rewrite(this.language, select);
                var reverted = select.OrderBy.Select(ob => new OrderExpression(ob.OrderType == OrderType.Ascending ? OrderType.Descending : OrderType.Ascending, ob.Expression));
                select = select.SetOrderBy(reverted);
                select = select.SetTake(null);
            }

            return select;
        }
    }
}