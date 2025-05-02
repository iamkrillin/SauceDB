// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Duplicate the query expression by making a copy with new table aliases
    /// </summary>
    public class QueryDuplicator : DbExpressionVisitor
    {
        Dictionary<TableAlias, TableAlias> map = new Dictionary<TableAlias, TableAlias>();

        public static Expression Duplicate(Expression expression)
        {
            return new QueryDuplicator().Visit(expression);
        }

        protected override Expression VisitTable(TableExpression table)
        {
            TableAlias newAlias = new TableAlias();
            this.map[table.Alias] = newAlias;
            return new TableExpression(newAlias, table.Entity, table.Name);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            TableAlias newAlias = new TableAlias();
            this.map[select.Alias] = newAlias;
            select = (SelectExpression)base.VisitSelect(select);
            return new SelectExpression(newAlias, select.Columns, select.From, select.Where, select.OrderBy, select.GroupBy, select.IsDistinct, select.Skip, select.Take, select.IsReverse);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            TableAlias newAlias;
            if (this.map.TryGetValue(column.Alias, out newAlias))
            {
                return new ColumnExpression(column.Type, newAlias, column.Name);
            }
            return column;
        }
    }
}