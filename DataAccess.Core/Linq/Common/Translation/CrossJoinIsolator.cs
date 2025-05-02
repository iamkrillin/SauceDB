// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Isolates cross joins from other types of joins using nested sub queries
    /// </summary>
    public class CrossJoinIsolator : DbExpressionVisitor
    {
        ILookup<TableAlias, ColumnExpression> columns;
        Dictionary<ColumnExpression, ColumnExpression> map = new Dictionary<ColumnExpression, ColumnExpression>();
        JoinType? lastJoin;

        /// <summary>
        /// Isolates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression Isolate(Expression expression)
        {
            return new CrossJoinIsolator().Visit(expression);
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            var saveColumns = this.columns;
            this.columns = ReferencedColumnGatherer.Gather(select).ToLookup(c => c.Alias);
            var saveLastJoin = this.lastJoin;
            this.lastJoin = null;
            var result = base.VisitSelect(select);
            this.columns = saveColumns;
            this.lastJoin = saveLastJoin;
            return result;
        }

        /// <summary>
        /// Visits the join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns></returns>
        protected override Expression VisitJoin(JoinExpression join)
        {
            var saveLastJoin = this.lastJoin;
            this.lastJoin = join.Join;
            join = (JoinExpression)base.VisitJoin(join);
            this.lastJoin = saveLastJoin;

            if (this.lastJoin != null && (join.Join == JoinType.CrossJoin) != (this.lastJoin == JoinType.CrossJoin))
            {
                var result = this.MakeSubquery(join);
                return result;
            }
            return join;
        }

        private bool IsCrossJoin(Expression expression)
        {
            var jex = expression as JoinExpression;
            if (jex != null)
            {
                return jex.Join == JoinType.CrossJoin;
            }
            return false;
        }

        private Expression MakeSubquery(Expression expression)
        {
            var newAlias = new TableAlias();
            var aliases = DeclaredAliasGatherer.Gather(expression);

            var decls = new List<ColumnDeclaration>();
            foreach (var ta in aliases)
            {
                foreach (var col in this.columns[ta])
                {
                    string name = decls.GetAvailableColumnName(col.Name);
                    var decl = new ColumnDeclaration(name, col);
                    decls.Add(decl);
                    var newCol = new ColumnExpression(col.Type, newAlias, col.Name);
                    this.map.Add(col, newCol);
                }
            }

            return new SelectExpression(newAlias, decls, expression, null);
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            ColumnExpression mapped;
            if (this.map.TryGetValue(column, out mapped))
            {
                return mapped;
            }
            return column;
        }
    }
}