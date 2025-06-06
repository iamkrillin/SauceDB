﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#pragma warning disable 1591

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Removes duplicate column declarations that refer to the same underlying column
    /// </summary>
    public class RedundantColumnRemover : DbExpressionVisitor
    {
        Dictionary<ColumnExpression, ColumnExpression> map;

        private RedundantColumnRemover()
        {
            this.map = new Dictionary<ColumnExpression, ColumnExpression>();
        }

        public static Expression Remove(Expression expression)
        {
            return new RedundantColumnRemover().Visit(expression);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            ColumnExpression mapped;
            if (this.map.TryGetValue(column, out mapped))
            {
                return mapped;
            }
            return column;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            select = (SelectExpression)base.VisitSelect(select);

            // look for redundant column declarations
            List<ColumnDeclaration> cols = select.Columns.OrderBy(c => c.Name).ToList();
            BitArray removed = new BitArray(select.Columns.Count);
            bool anyRemoved = false;
            for (int i = 0, n = cols.Count; i < n - 1; i++)
            {
                ColumnDeclaration ci = cols[i];
                ColumnExpression cix = ci.Expression as ColumnExpression;
                ColumnExpression cxi = new ColumnExpression(ci.Expression.Type, select.Alias, ci.Name);
                for (int j = i + 1; j < n; j++)
                {
                    if (!removed.Get(j))
                    {
                        ColumnDeclaration cj = cols[j];
                        if (SameExpression(ci.Expression, cj.Expression))
                        {
                            ColumnExpression cxj = new ColumnExpression(cj.Expression.Type, select.Alias, cj.Name);
                            this.map.Add(cxj, cxi);
                            removed.Set(j, true);
                            anyRemoved = true;
                        }
                    }
                }
            }
            if (anyRemoved)
            {
                List<ColumnDeclaration> newDecls = new List<ColumnDeclaration>();
                for (int i = 0, n = cols.Count; i < n; i++)
                {
                    if (!removed.Get(i))
                    {
                        newDecls.Add(cols[i]);
                    }
                }
                select = select.SetColumns(newDecls);
            }
            return select;
        }

        bool SameExpression(Expression a, Expression b)
        {
            if (a == b) return true;
            ColumnExpression ca = a as ColumnExpression;
            ColumnExpression cb = b as ColumnExpression;
            return (ca != null && cb != null && ca.Alias == cb.Alias && ca.Name == cb.Name);
        }
    }
}