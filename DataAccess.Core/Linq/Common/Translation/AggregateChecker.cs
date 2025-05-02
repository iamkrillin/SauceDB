// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// Determines if a SelectExpression contains any aggregate expressions
    /// </summary>
    internal class AggregateChecker : DbExpressionVisitor
    {
        bool hasAggregate = false;
        private AggregateChecker()
        {
        }

        internal static bool HasAggregates(SelectExpression expression)
        {
            AggregateChecker checker = new AggregateChecker();
            checker.Visit(expression);
            return checker.hasAggregate;
        }

        /// <summary>
        /// Visits the aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            this.hasAggregate = true;
            return aggregate;
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            // only consider aggregates in these locations
            this.Visit(select.Where);
            this.VisitOrderBy(select.OrderBy);
            this.VisitColumnDeclarations(select.Columns);
            return select;
        }

        /// <summary>
        /// Visits the subquery.
        /// </summary>
        /// <param name="subquery">The subquery.</param>
        /// <returns></returns>
        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            // don't count aggregates in subqueries
            return subquery;
        }
    }
}