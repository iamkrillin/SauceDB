// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Translation;
using DataAccess.Core.Linq.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
#pragma warning disable 1591

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// Determines if two expressions are equivalent. Supports DbExpression nodes.
    /// </summary>
    public class DbExpressionComparer : ExpressionComparer
    {
        ScopedDictionary<TableAlias, TableAlias> aliasScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExpressionComparer"/> class.
        /// </summary>
        /// <param name="parameterScope">The parameter scope.</param>
        /// <param name="fnCompare">The fn compare.</param>
        /// <param name="aliasScope">The alias scope.</param>
        protected DbExpressionComparer(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, Func<object, object, bool> fnCompare, ScopedDictionary<TableAlias, TableAlias> aliasScope)
            : base(parameterScope, fnCompare)
        {
            this.aliasScope = aliasScope;
        }

        /// <summary>
        /// Ares the equal.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public new static bool AreEqual(Expression a, Expression b)
        {
            return AreEqual(null, null, a, b, null);
        }

        /// <summary>
        /// Ares the equal.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="fnCompare">The fn compare.</param>
        /// <returns></returns>
        public new static bool AreEqual(Expression a, Expression b, Func<object, object, bool> fnCompare)
        {
            return AreEqual(null, null, a, b, fnCompare);
        }

        /// <summary>
        /// Ares the equal.
        /// </summary>
        /// <param name="parameterScope">The parameter scope.</param>
        /// <param name="aliasScope">The alias scope.</param>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, ScopedDictionary<TableAlias, TableAlias> aliasScope, Expression a, Expression b)
        {
            return new DbExpressionComparer(parameterScope, null, aliasScope).Compare(a, b);
        }

        /// <summary>
        /// Ares the equal.
        /// </summary>
        /// <param name="parameterScope">The parameter scope.</param>
        /// <param name="aliasScope">The alias scope.</param>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="fnCompare">The fn compare.</param>
        /// <returns></returns>
        public static bool AreEqual(ScopedDictionary<ParameterExpression, ParameterExpression> parameterScope, ScopedDictionary<TableAlias, TableAlias> aliasScope, Expression a, Expression b, Func<object, object, bool> fnCompare)
        {
            return new DbExpressionComparer(parameterScope, fnCompare, aliasScope).Compare(a, b);
        }

        /// <summary>
        /// Compares the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected override bool Compare(Expression a, Expression b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.NodeType != b.NodeType)
                return false;
            if (a.Type != b.Type)
                return false;
            switch ((DbExpressionType)a.NodeType)
            {
                case DbExpressionType.Table:
                    return this.CompareTable((TableExpression)a, (TableExpression)b);
                case DbExpressionType.Column:
                    return this.CompareColumn((ColumnExpression)a, (ColumnExpression)b);
                case DbExpressionType.Select:
                    return this.CompareSelect((SelectExpression)a, (SelectExpression)b);
                case DbExpressionType.Join:
                    return this.CompareJoin((JoinExpression)a, (JoinExpression)b);
                case DbExpressionType.Aggregate:
                    return this.CompareAggregate((AggregateExpression)a, (AggregateExpression)b);
                case DbExpressionType.Scalar:
                case DbExpressionType.Exists:
                case DbExpressionType.In:
                    return this.CompareSubquery((SubqueryExpression)a, (SubqueryExpression)b);
                case DbExpressionType.AggregateSubquery:
                    return this.CompareAggregateSubquery((AggregateSubqueryExpression)a, (AggregateSubqueryExpression)b);
                case DbExpressionType.IsNull:
                    return this.CompareIsNull((IsNullExpression)a, (IsNullExpression)b);
                case DbExpressionType.Between:
                    return this.CompareBetween((BetweenExpression)a, (BetweenExpression)b);
                case DbExpressionType.RowCount:
                    return this.CompareRowNumber((RowNumberExpression)a, (RowNumberExpression)b);
                case DbExpressionType.Projection:
                    return this.CompareProjection((ProjectionExpression)a, (ProjectionExpression)b);
                case DbExpressionType.NamedValue:
                    return this.CompareNamedValue((NamedValueExpression)a, (NamedValueExpression)b);
                case DbExpressionType.Batch:
                    return this.CompareBatch((BatchExpression)a, (BatchExpression)b);
                case DbExpressionType.Function:
                    return this.CompareFunction((FunctionExpression)a, (FunctionExpression)b);
                case DbExpressionType.Entity:
                    return this.CompareEntity((EntityExpression)a, (EntityExpression)b);
                case DbExpressionType.If:
                    return this.CompareIf((IFCommand)a, (IFCommand)b);
                case DbExpressionType.Block:
                    return this.CompareBlock((BlockCommand)a, (BlockCommand)b);
                default:
                    return base.Compare(a, b);
            }
        }

        /// <summary>
        /// Compares the table.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareTable(TableExpression a, TableExpression b)
        {
            return a.Name.Equals(b.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compares the column.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareColumn(ColumnExpression a, ColumnExpression b)
        {
            return this.CompareAlias(a.Alias, b.Alias) && a.Name.Equals(b.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Compares the alias.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareAlias(TableAlias a, TableAlias b)
        {
            if (this.aliasScope != null)
            {
                TableAlias mapped;
                if (this.aliasScope.TryGetValue(a, out mapped))
                    return mapped == b;
            }
            return a == b;
        }

        /// <summary>
        /// Compares the select.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareSelect(SelectExpression a, SelectExpression b)
        {
            var save = this.aliasScope;
            try
            {
                if (!this.Compare(a.From, b.From))
                    return false;

                this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(save);
                this.MapAliases(a.From, b.From);

                return this.Compare(a.Where, b.Where) && this.CompareOrderList(a.OrderBy, b.OrderBy) && this.CompareExpressionList(a.GroupBy, b.GroupBy)
                        && this.Compare(a.Skip, b.Skip) && this.Compare(a.Take, b.Take) && a.IsDistinct == b.IsDistinct && a.IsReverse == b.IsReverse && this.CompareColumnDeclarations(a.Columns, b.Columns);
            }
            finally
            {
                this.aliasScope = save;
            }
        }

        /// <summary>
        /// Maps the aliases.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        private void MapAliases(Expression a, Expression b)
        {
            TableAlias[] prodA = DeclaredAliasGatherer.Gather(a).ToArray();
            TableAlias[] prodB = DeclaredAliasGatherer.Gather(b).ToArray();
            for (int i = 0, n = prodA.Length; i < n; i++)
            {
                this.aliasScope.Add(prodA[i], prodB[i]);
            }
        }

        /// <summary>
        /// Compares the order list.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareOrderList(ReadOnlyCollection<OrderExpression> a, ReadOnlyCollection<OrderExpression> b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (a[i].OrderType != b[i].OrderType || !this.Compare(a[i].Expression, b[i].Expression))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the column declarations.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> a, ReadOnlyCollection<ColumnDeclaration> b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            if (a.Count != b.Count)
                return false;

            for (int i = 0, n = a.Count; i < n; i++)
            {
                if (!this.CompareColumnDeclaration(a[i], b[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool CompareColumnDeclaration(ColumnDeclaration a, ColumnDeclaration b)
        {
            return a.Name.Equals(b.Name, StringComparison.InvariantCultureIgnoreCase) && this.Compare(a.Expression, b.Expression);
        }

        /// <summary>
        /// Compares the join.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareJoin(JoinExpression a, JoinExpression b)
        {
            if (a.Join != b.Join || !this.Compare(a.Left, b.Left))
                return false;

            if (a.Join == JoinType.CrossApply || a.Join == JoinType.OuterApply)
            {
                var save = this.aliasScope;
                try
                {
                    this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(this.aliasScope);
                    this.MapAliases(a.Left, b.Left);

                    return this.Compare(a.Right, b.Right) && this.Compare(a.Condition, b.Condition);
                }
                finally
                {
                    this.aliasScope = save;
                }
            }
            else
            {
                return this.Compare(a.Right, b.Right) && this.Compare(a.Condition, b.Condition);
            }
        }

        /// <summary>
        /// Compares the aggregate.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareAggregate(AggregateExpression a, AggregateExpression b)
        {
            return a.AggregateName.Equals(b.AggregateName, StringComparison.InvariantCultureIgnoreCase) && this.Compare(a.Argument, b.Argument);
        }

        /// <summary>
        /// Compares the is null.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareIsNull(IsNullExpression a, IsNullExpression b)
        {
            return this.Compare(a.Expression, b.Expression);
        }

        /// <summary>
        /// Compares the between.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareBetween(BetweenExpression a, BetweenExpression b)
        {
            return this.Compare(a.Expression, b.Expression) && this.Compare(a.Lower, b.Lower) && this.Compare(a.Upper, b.Upper);
        }

        /// <summary>
        /// Compares the row number.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareRowNumber(RowNumberExpression a, RowNumberExpression b)
        {
            return this.CompareOrderList(a.OrderBy, b.OrderBy);
        }

        /// <summary>
        /// Compares the named value.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareNamedValue(NamedValueExpression a, NamedValueExpression b)
        {
            return a.Name.Equals(b.Name, StringComparison.InvariantCultureIgnoreCase) && this.Compare(a.Value, b.Value);
        }

        /// <summary>
        /// Compares the subquery.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareSubquery(SubqueryExpression a, SubqueryExpression b)
        {
            if (a.NodeType != b.NodeType)
                return false;
            switch ((DbExpressionType)a.NodeType)
            {
                case DbExpressionType.Scalar:
                    return this.CompareScalar((ScalarExpression)a, (ScalarExpression)b);
                case DbExpressionType.Exists:
                    return this.CompareExists((ExistsExpression)a, (ExistsExpression)b);
                case DbExpressionType.In:
                    return this.CompareIn((InExpression)a, (InExpression)b);
            }
            return false;
        }

        /// <summary>
        /// Compares the scalar.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareScalar(ScalarExpression a, ScalarExpression b)
        {
            return this.Compare(a.Select, b.Select);
        }

        /// <summary>
        /// Compares the exists.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareExists(ExistsExpression a, ExistsExpression b)
        {
            return this.Compare(a.Select, b.Select);
        }

        /// <summary>
        /// Compares the in.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareIn(InExpression a, InExpression b)
        {
            return this.Compare(a.Expression, b.Expression) && this.Compare(a.Select, b.Select) && this.CompareExpressionList(a.Values, b.Values);
        }

        /// <summary>
        /// Compares the aggregate subquery.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareAggregateSubquery(AggregateSubqueryExpression a, AggregateSubqueryExpression b)
        {
            return this.Compare(a.AggregateAsSubquery, b.AggregateAsSubquery) && this.Compare(a.AggregateInGroupSelect, b.AggregateInGroupSelect) && a.GroupByAlias == b.GroupByAlias;
        }

        /// <summary>
        /// Compares the projection.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual bool CompareProjection(ProjectionExpression a, ProjectionExpression b)
        {
            if (!this.Compare(a.Select, b.Select))
                return false;

            var save = this.aliasScope;
            try
            {
                this.aliasScope = new ScopedDictionary<TableAlias, TableAlias>(this.aliasScope);
                this.aliasScope.Add(a.Select.Alias, b.Select.Alias);

                return this.Compare(a.Projector, b.Projector) && this.Compare(a.Aggregator, b.Aggregator) && a.IsSingleton == b.IsSingleton;
            }
            finally
            {
                this.aliasScope = save;
            }
        }

        /// <summary>
        /// Compares the column assignments.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual bool CompareColumnAssignments(ReadOnlyCollection<ColumnAssignment> x, ReadOnlyCollection<ColumnAssignment> y)
        {
            if (x == y)
                return true;
            if (x.Count != y.Count)
                return false;
            for (int i = 0, n = x.Count; i < n; i++)
            {
                if (!this.Compare(x[i].Column, y[i].Column) || !this.Compare(x[i].Expression, y[i].Expression))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the batch.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual bool CompareBatch(BatchExpression x, BatchExpression y)
        {
            return this.Compare(x.Input, y.Input) && this.Compare(x.Operation, y.Operation) && this.Compare(x.BatchSize, y.BatchSize) && this.Compare(x.Stream, y.Stream);
        }

        /// <summary>
        /// Compares if.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual bool CompareIf(IFCommand x, IFCommand y)
        {
            return this.Compare(x.Check, y.Check) && this.Compare(x.IfTrue, y.IfTrue) && this.Compare(x.IfFalse, y.IfFalse);
        }

        /// <summary>
        /// Compares the block.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual bool CompareBlock(BlockCommand x, BlockCommand y)
        {
            if (x.Commands.Count != y.Commands.Count)
                return false;
            for (int i = 0, n = x.Commands.Count; i < n; i++)
            {
                if (!this.Compare(x.Commands[i], y.Commands[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Compares the function.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual bool CompareFunction(FunctionExpression x, FunctionExpression y)
        {
            return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase) && this.CompareExpressionList(x.Arguments, y.Arguments);
        }

        /// <summary>
        /// Compares the entity.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        protected virtual bool CompareEntity(EntityExpression x, EntityExpression y)
        {
            return x.Entity == y.Entity && this.Compare(x.Expression, y.Expression);
        }
    }
}