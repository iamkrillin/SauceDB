// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using DataAccess.Core.Linq.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// An extended expression visitor including custom DbExpression nodes
    /// </summary>
    public abstract class DbExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Table:
                    return this.VisitTable((TableExpression)exp);
                case DbExpressionType.Column:
                    return this.VisitColumn((ColumnExpression)exp);
                case DbExpressionType.Select:
                    return this.VisitSelect((SelectExpression)exp);
                case DbExpressionType.Join:
                    return this.VisitJoin((JoinExpression)exp);
                case DbExpressionType.OuterJoined:
                    return this.VisitOuterJoined((OuterJoinedExpression)exp);
                case DbExpressionType.Aggregate:
                    return this.VisitAggregate((AggregateExpression)exp);
                case DbExpressionType.Scalar:
                case DbExpressionType.Exists:
                case DbExpressionType.In:
                    return this.VisitSubquery((SubqueryExpression)exp);
                case DbExpressionType.AggregateSubquery:
                    return this.VisitAggregateSubquery((AggregateSubqueryExpression)exp);
                case DbExpressionType.IsNull:
                    return this.VisitIsNull((IsNullExpression)exp);
                case DbExpressionType.Between:
                    return this.VisitBetween((BetweenExpression)exp);
                case DbExpressionType.RowCount:
                    return this.VisitRowNumber((RowNumberExpression)exp);
                case DbExpressionType.Projection:
                    return this.VisitProjection((ProjectionExpression)exp);
                case DbExpressionType.NamedValue:
                    return this.VisitNamedValue((NamedValueExpression)exp);
                case DbExpressionType.ClientJoin:
                    return this.VisitClientJoin((ClientJoinExpression)exp);
                case DbExpressionType.If:
                case DbExpressionType.Block:
                case DbExpressionType.Declaration:
                    return this.VisitCommand((CommandExpression)exp);
                case DbExpressionType.Batch:
                    return this.VisitBatch((BatchExpression)exp);
                case DbExpressionType.Variable:
                    return this.VisitVariable((VariableExpression)exp);
                case DbExpressionType.Function:
                    return this.VisitFunction((FunctionExpression)exp);
                case DbExpressionType.Entity:
                    return this.VisitEntity((EntityExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        /// <summary>
        /// Visits the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected virtual Expression VisitEntity(EntityExpression entity)
        {
            var exp = this.Visit(entity.Expression);
            return this.UpdateEntity(entity, exp);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected EntityExpression UpdateEntity(EntityExpression entity, Expression expression)
        {
            if (expression != entity.Expression)
            {
                return new EntityExpression(entity.Entity, expression);
            }
            return entity;
        }

        /// <summary>
        /// Visits the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        protected virtual Expression VisitTable(TableExpression table)
        {
            return table;
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            return column;
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected virtual Expression VisitSelect(SelectExpression select)
        {
            var from = this.VisitSource(select.From);
            var where = this.Visit(select.Where);
            var orderBy = this.VisitOrderBy(select.OrderBy);
            var groupBy = this.VisitExpressionList(select.GroupBy);
            var skip = this.Visit(select.Skip);
            var take = this.Visit(select.Take);
            var columns = this.VisitColumnDeclarations(select.Columns);
            return this.UpdateSelect(select, from, where, orderBy, groupBy, skip, take, select.IsDistinct, select.IsReverse, columns);
        }

        /// <summary>
        /// Updates the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <param name="from">From.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="groupBy">The group by.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <param name="isReverse">if set to <c>true</c> [is reverse].</param>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        protected SelectExpression UpdateSelect(SelectExpression select, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy, Expression skip, Expression take, bool isDistinct, bool isReverse, IEnumerable<ColumnDeclaration> columns)
        {
            if (from != select.From || where != select.Where || orderBy != select.OrderBy || groupBy != select.GroupBy || take != select.Take ||
                          skip != select.Skip || isDistinct != select.IsDistinct || columns != select.Columns || isReverse != select.IsReverse)
            {
                return new SelectExpression(select.Alias, columns, from, where, orderBy, groupBy, isDistinct, skip, take, isReverse);
            }
            return select;
        }

        /// <summary>
        /// Visits the join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns></returns>
        protected virtual Expression VisitJoin(JoinExpression join)
        {
            var left = this.VisitSource(join.Left);
            var right = this.VisitSource(join.Right);
            var condition = this.Visit(join.Condition);
            return this.UpdateJoin(join, join.Join, left, right, condition);
        }

        /// <summary>
        /// Updates the join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        protected JoinExpression UpdateJoin(JoinExpression join, JoinType joinType, Expression left, Expression right, Expression condition)
        {
            if (joinType != join.Join || left != join.Left || right != join.Right || condition != join.Condition)
                return new JoinExpression(joinType, left, right, condition);

            return join;
        }

        /// <summary>
        /// Visits the outer joined.
        /// </summary>
        /// <param name="outer">The outer.</param>
        /// <returns></returns>
        protected virtual Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            var test = this.Visit(outer.Test);
            var expression = this.Visit(outer.Expression);
            return this.UpdateOuterJoined(outer, test, expression);
        }

        /// <summary>
        /// Updates the outer joined.
        /// </summary>
        /// <param name="outer">The outer.</param>
        /// <param name="test">The test.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected OuterJoinedExpression UpdateOuterJoined(OuterJoinedExpression outer, Expression test, Expression expression)
        {
            if (test != outer.Test || expression != outer.Expression)
                return new OuterJoinedExpression(test, expression);

            return outer;
        }

        /// <summary>
        /// Visits the aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        protected virtual Expression VisitAggregate(AggregateExpression aggregate)
        {
            var arg = this.Visit(aggregate.Argument);
            return this.UpdateAggregate(aggregate, aggregate.Type, aggregate.AggregateName, arg, aggregate.IsDistinct);
        }

        /// <summary>
        /// Updates the aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <param name="type">The type.</param>
        /// <param name="aggType">Type of the agg.</param>
        /// <param name="arg">The arg.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <returns></returns>
        protected AggregateExpression UpdateAggregate(AggregateExpression aggregate, Type type, string aggType, Expression arg, bool isDistinct)
        {
            if (type != aggregate.Type || aggType != aggregate.AggregateName || arg != aggregate.Argument || isDistinct != aggregate.IsDistinct)
                return new AggregateExpression(type, aggType, arg, isDistinct);

            return aggregate;
        }

        /// <summary>
        /// Visits the is null.
        /// </summary>
        /// <param name="isnull">The isnull.</param>
        /// <returns></returns>
        protected virtual Expression VisitIsNull(IsNullExpression isnull)
        {
            var expr = this.Visit(isnull.Expression);
            return this.UpdateIsNull(isnull, expr);
        }

        /// <summary>
        /// Updates the is null.
        /// </summary>
        /// <param name="isnull">The isnull.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected IsNullExpression UpdateIsNull(IsNullExpression isnull, Expression expression)
        {
            if (expression != isnull.Expression)
                return new IsNullExpression(expression);

            return isnull;
        }

        /// <summary>
        /// Visits the between.
        /// </summary>
        /// <param name="between">The between.</param>
        /// <returns></returns>
        protected virtual Expression VisitBetween(BetweenExpression between)
        {
            var expr = this.Visit(between.Expression);
            var lower = this.Visit(between.Lower);
            var upper = this.Visit(between.Upper);
            return this.UpdateBetween(between, expr, lower, upper);
        }

        /// <summary>
        /// Updates the between.
        /// </summary>
        /// <param name="between">The between.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <returns></returns>
        protected BetweenExpression UpdateBetween(BetweenExpression between, Expression expression, Expression lower, Expression upper)
        {
            if (expression != between.Expression || lower != between.Lower || upper != between.Upper)
                return new BetweenExpression(expression, lower, upper);

            return between;
        }

        /// <summary>
        /// Visits the row number.
        /// </summary>
        /// <param name="rowNumber">The row number.</param>
        /// <returns></returns>
        protected virtual Expression VisitRowNumber(RowNumberExpression rowNumber)
        {
            var orderby = this.VisitOrderBy(rowNumber.OrderBy);
            return this.UpdateRowNumber(rowNumber, orderby);
        }

        /// <summary>
        /// Updates the row number.
        /// </summary>
        /// <param name="rowNumber">The row number.</param>
        /// <param name="orderBy">The order by.</param>
        /// <returns></returns>
        protected RowNumberExpression UpdateRowNumber(RowNumberExpression rowNumber, IEnumerable<OrderExpression> orderBy)
        {
            if (orderBy != rowNumber.OrderBy)
                return new RowNumberExpression(orderBy);

            return rowNumber;
        }

        /// <summary>
        /// Visits the named value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual Expression VisitNamedValue(NamedValueExpression value)
        {
            return value;
        }

        /// <summary>
        /// Visits the subquery.
        /// </summary>
        /// <param name="subquery">The subquery.</param>
        /// <returns></returns>
        protected virtual Expression VisitSubquery(SubqueryExpression subquery)
        {
            switch ((DbExpressionType)subquery.NodeType)
            {
                case DbExpressionType.Scalar:
                    return this.VisitScalar((ScalarExpression)subquery);
                case DbExpressionType.Exists:
                    return this.VisitExists((ExistsExpression)subquery);
                case DbExpressionType.In:
                    return this.VisitIn((InExpression)subquery);
            }
            return subquery;
        }

        /// <summary>
        /// Visits the scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <returns></returns>
        protected virtual Expression VisitScalar(ScalarExpression scalar)
        {
            var select = (SelectExpression)this.Visit(scalar.Select);
            return this.UpdateScalar(scalar, select);
        }

        /// <summary>
        /// Updates the scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected ScalarExpression UpdateScalar(ScalarExpression scalar, SelectExpression select)
        {
            if (select != scalar.Select)
                return new ScalarExpression(scalar.Type, select);

            return scalar;
        }

        /// <summary>
        /// Visits the exists.
        /// </summary>
        /// <param name="exists">The exists.</param>
        /// <returns></returns>
        protected virtual Expression VisitExists(ExistsExpression exists)
        {
            var select = (SelectExpression)this.Visit(exists.Select);
            return this.UpdateExists(exists, select);
        }

        /// <summary>
        /// Updates the exists.
        /// </summary>
        /// <param name="exists">The exists.</param>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected ExistsExpression UpdateExists(ExistsExpression exists, SelectExpression select)
        {
            if (select != exists.Select)
                return new ExistsExpression(select);

            return exists;
        }

        /// <summary>
        /// Visits the in.
        /// </summary>
        /// <param name="in">The @in.</param>
        /// <returns></returns>
        protected virtual Expression VisitIn(InExpression @in)
        {
            var expr = this.Visit(@in.Expression);
            var select = (SelectExpression)this.Visit(@in.Select);
            var values = this.VisitExpressionList(@in.Values);
            return this.UpdateIn(@in, expr, select, values);
        }

        /// <summary>
        /// Updates the in.
        /// </summary>
        /// <param name="in">The @in.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="select">The select.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        protected InExpression UpdateIn(InExpression @in, Expression expression, SelectExpression select, IEnumerable<Expression> values)
        {
            if (expression != @in.Expression || select != @in.Select || values != @in.Values)
            {
                if (select != null)
                    return new InExpression(expression, select);
                else
                    return new InExpression(expression, values);
            }
            return @in;
        }

        /// <summary>
        /// Visits the aggregate subquery.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        protected virtual Expression VisitAggregateSubquery(AggregateSubqueryExpression aggregate)
        {
            var subquery = (ScalarExpression)this.Visit(aggregate.AggregateAsSubquery);
            return this.UpdateAggregateSubquery(aggregate, subquery);
        }

        /// <summary>
        /// Updates the aggregate subquery.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <param name="subquery">The subquery.</param>
        /// <returns></returns>
        protected AggregateSubqueryExpression UpdateAggregateSubquery(AggregateSubqueryExpression aggregate, ScalarExpression subquery)
        {
            if (subquery != aggregate.AggregateAsSubquery)
                return new AggregateSubqueryExpression(aggregate.GroupByAlias, aggregate.AggregateInGroupSelect, subquery);

            return aggregate;
        }

        /// <summary>
        /// Visits the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected virtual Expression VisitSource(Expression source)
        {
            return this.Visit(source);
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        protected virtual Expression VisitProjection(ProjectionExpression proj)
        {
            var select = (SelectExpression)this.Visit(proj.Select);
            var projector = this.Visit(proj.Projector);
            return this.UpdateProjection(proj, select, projector, proj.Aggregator);
        }

        /// <summary>
        /// Updates the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <param name="select">The select.</param>
        /// <param name="projector">The projector.</param>
        /// <param name="aggregator">The aggregator.</param>
        /// <returns></returns>
        protected ProjectionExpression UpdateProjection(ProjectionExpression proj, SelectExpression select, Expression projector, LambdaExpression aggregator)
        {
            if (select != proj.Select || projector != proj.Projector || aggregator != proj.Aggregator)
                return new ProjectionExpression(select, projector, aggregator);

            return proj;
        }

        /// <summary>
        /// Visits the client join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns></returns>
        protected virtual Expression VisitClientJoin(ClientJoinExpression join)
        {
            var projection = (ProjectionExpression)this.Visit(join.Projection);
            var outerKey = this.VisitExpressionList(join.OuterKey);
            var innerKey = this.VisitExpressionList(join.InnerKey);
            return this.UpdateClientJoin(join, projection, outerKey, innerKey);
        }

        /// <summary>
        /// Updates the client join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <param name="projection">The projection.</param>
        /// <param name="outerKey">The outer key.</param>
        /// <param name="innerKey">The inner key.</param>
        /// <returns></returns>
        protected ClientJoinExpression UpdateClientJoin(ClientJoinExpression join, ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
        {
            if (projection != join.Projection || outerKey != join.OuterKey || innerKey != join.InnerKey)
                return new ClientJoinExpression(projection, outerKey, innerKey);

            return join;
        }

        /// <summary>
        /// Visits the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected virtual Expression VisitCommand(CommandExpression command)
        {
            switch ((DbExpressionType)command.NodeType)
            {
                case DbExpressionType.If:
                    return this.VisitIf((IFCommand)command);
                case DbExpressionType.Block:
                    return this.VisitBlock((BlockCommand)command);
                case DbExpressionType.Declaration:
                    return this.VisitDeclaration((DeclarationCommand)command);
                default:
                    return this.VisitUnknown(command);
            }
        }

        /// <summary>
        /// Visits the batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <returns></returns>
        protected virtual Expression VisitBatch(BatchExpression batch)
        {
            var operation = (LambdaExpression)this.Visit(batch.Operation);
            var batchSize = this.Visit(batch.BatchSize);
            var stream = this.Visit(batch.Stream);
            return this.UpdateBatch(batch, batch.Input, operation, batchSize, stream);
        }

        /// <summary>
        /// Updates the batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <param name="input">The input.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        protected BatchExpression UpdateBatch(BatchExpression batch, Expression input, LambdaExpression operation, Expression batchSize, Expression stream)
        {
            if (input != batch.Input || operation != batch.Operation || batchSize != batch.BatchSize || stream != batch.Stream)
                return new BatchExpression(input, operation, batchSize, stream);

            return batch;
        }

        /// <summary>
        /// Visits if.
        /// </summary>
        /// <param name="ifx">The ifx.</param>
        /// <returns></returns>
        protected virtual Expression VisitIf(IFCommand ifx)
        {
            var check = this.Visit(ifx.Check);
            var ifTrue = this.Visit(ifx.IfTrue);
            var ifFalse = this.Visit(ifx.IfFalse);
            return this.UpdateIf(ifx, check, ifTrue, ifFalse);
        }

        /// <summary>
        /// Updates if.
        /// </summary>
        /// <param name="ifx">The ifx.</param>
        /// <param name="check">The check.</param>
        /// <param name="ifTrue">If true.</param>
        /// <param name="ifFalse">If false.</param>
        /// <returns></returns>
        protected IFCommand UpdateIf(IFCommand ifx, Expression check, Expression ifTrue, Expression ifFalse)
        {
            if (check != ifx.Check || ifTrue != ifx.IfTrue || ifFalse != ifx.IfFalse)
                return new IFCommand(check, ifTrue, ifFalse);

            return ifx;
        }

        /// <summary>
        /// Visits the block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        protected virtual Expression VisitBlock(BlockCommand block)
        {
            var commands = this.VisitExpressionList(block.Commands);
            return this.UpdateBlock(block, commands);
        }

        /// <summary>
        /// Updates the block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        protected BlockCommand UpdateBlock(BlockCommand block, IList<Expression> commands)
        {
            if (block.Commands != commands)
                return new BlockCommand(commands);

            return block;
        }

        /// <summary>
        /// Visits the declaration.
        /// </summary>
        /// <param name="decl">The decl.</param>
        /// <returns></returns>
        protected virtual Expression VisitDeclaration(DeclarationCommand decl)
        {
            var variables = this.VisitVariableDeclarations(decl.Variables);
            var source = (SelectExpression)this.Visit(decl.Source);
            return this.UpdateDeclaration(decl, variables, source);

        }

        /// <summary>
        /// Updates the declaration.
        /// </summary>
        /// <param name="decl">The decl.</param>
        /// <param name="variables">The variables.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected DeclarationCommand UpdateDeclaration(DeclarationCommand decl, IEnumerable<VariableDeclaration> variables, SelectExpression source)
        {
            if (variables != decl.Variables || source != decl.Source)
                return new DeclarationCommand(variables, source);

            return decl;
        }

        /// <summary>
        /// Visits the variable.
        /// </summary>
        /// <param name="vex">The vex.</param>
        /// <returns></returns>
        protected virtual Expression VisitVariable(VariableExpression vex)
        {
            return vex;
        }

        /// <summary>
        /// Visits the function.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        protected virtual Expression VisitFunction(FunctionExpression func)
        {
            var arguments = this.VisitExpressionList(func.Arguments);
            return this.UpdateFunction(func, func.Name, arguments);
        }

        /// <summary>
        /// Updates the function.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        protected FunctionExpression UpdateFunction(FunctionExpression func, string name, IEnumerable<Expression> arguments)
        {
            if (name != func.Name || arguments != func.Arguments)
                return new FunctionExpression(func.Type, name, arguments);

            return func;
        }

        /// <summary>
        /// Visits the column assignment.
        /// </summary>
        /// <param name="ca">The ca.</param>
        /// <returns></returns>
        protected virtual ColumnAssignment VisitColumnAssignment(ColumnAssignment ca)
        {
            ColumnExpression c = (ColumnExpression)this.Visit(ca.Column);
            Expression e = this.Visit(ca.Expression);
            return this.UpdateColumnAssignment(ca, c, e);
        }

        protected ColumnAssignment UpdateColumnAssignment(ColumnAssignment ca, ColumnExpression c, Expression e)
        {
            if (c != ca.Column || e != ca.Expression)
                return new ColumnAssignment(c, e);

            return ca;
        }

        /// <summary>
        /// Visits the column assignments.
        /// </summary>
        /// <param name="assignments">The assignments.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<ColumnAssignment> VisitColumnAssignments(ReadOnlyCollection<ColumnAssignment> assignments)
        {
            List<ColumnAssignment> alternate = null;
            for (int i = 0, n = assignments.Count; i < n; i++)
            {
                ColumnAssignment assignment = this.VisitColumnAssignment(assignments[i]);
                if (alternate == null && assignment != assignments[i])
                    alternate = assignments.Take(i).ToList();

                if (alternate != null)
                    alternate.Add(assignment);
            }

            if (alternate != null)
                return alternate.AsReadOnly();

            return assignments;
        }

        /// <summary>
        /// Visits the column declarations.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<ColumnDeclaration> VisitColumnDeclarations(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            List<ColumnDeclaration> alternate = null;
            for (int i = 0, n = columns.Count; i < n; i++)
            {
                ColumnDeclaration column = columns[i];
                Expression e = this.Visit(column.Expression);
                if (alternate == null && e != column.Expression)
                    alternate = columns.Take(i).ToList();

                if (alternate != null)
                    alternate.Add(new ColumnDeclaration(column.Name, e));
            }

            if (alternate != null)
                return alternate.AsReadOnly();

            return columns;
        }

        /// <summary>
        /// Visits the variable declarations.
        /// </summary>
        /// <param name="decls">The decls.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<VariableDeclaration> VisitVariableDeclarations(ReadOnlyCollection<VariableDeclaration> decls)
        {
            List<VariableDeclaration> alternate = null;
            for (int i = 0, n = decls.Count; i < n; i++)
            {
                VariableDeclaration decl = decls[i];
                Expression e = this.Visit(decl.Expression);
                if (alternate == null && e != decl.Expression)
                    alternate = decls.Take(i).ToList();

                if (alternate != null)
                    alternate.Add(new VariableDeclaration(decl.Name, e));
            }

            if (alternate != null)
                return alternate.AsReadOnly();

            return decls;
        }

        /// <summary>
        /// Visits the order by.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<OrderExpression> VisitOrderBy(ReadOnlyCollection<OrderExpression> expressions)
        {
            if (expressions != null)
            {
                List<OrderExpression> alternate = null;
                for (int i = 0, n = expressions.Count; i < n; i++)
                {
                    OrderExpression expr = expressions[i];
                    Expression e = this.Visit(expr.Expression);
                    if (alternate == null && e != expr.Expression)
                        alternate = expressions.Take(i).ToList();

                    if (alternate != null)
                        alternate.Add(new OrderExpression(expr.OrderType, e));
                }

                if (alternate != null)
                    return alternate.AsReadOnly();
            }
            return expressions;
        }
    }
}