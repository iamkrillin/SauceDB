// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common.Mapping;
using DataAccess.Core.Linq.Common.Translation;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Builds an execution plan for a query expression
    /// </summary>
    public class ExecutionBuilder : DbExpressionVisitor
    {
        QueryPolicy policy;
        QueryLinguist linguist;
        Expression executor;
        Scope scope;
        bool isTop = true;
        MemberInfo receivingMember;
        int nReaders = 0;
        List<ParameterExpression> variables = new List<ParameterExpression>();
        List<Expression> initializers = new List<Expression>();
        Dictionary<string, Expression> variableMap = new Dictionary<string, Expression>();

        /// <summary>
        /// Prevents a default instance of the <see cref="ExecutionBuilder"/> class from being created.
        /// </summary>
        /// <param name="linguist">The linguist.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="executor">The executor.</param>
        private ExecutionBuilder(QueryLinguist linguist, QueryPolicy policy, Expression executor)
        {
            this.linguist = linguist;
            this.policy = policy;
            this.executor = executor;
        }

        /// <summary>
        /// Builds the specified linguist.
        /// </summary>
        /// <param name="linguist">The linguist.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static Expression Build(QueryLinguist linguist, QueryPolicy policy, Expression expression, Expression provider)
        {
            var executor = Expression.Parameter(typeof(QueryExecutor), "executor");
            var builder = new ExecutionBuilder(linguist, policy, executor);
            builder.variables.Add(executor);
            builder.initializers.Add(Expression.Call(Expression.Convert(provider, typeof(ICreateExecutor)), "CreateExecutor", null, null));
            var result = builder.Build(expression);
            return result;
        }

        /// <summary>
        /// Builds the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private Expression Build(Expression expression)
        {
            expression = this.Visit(expression);
            expression = this.AddVariables(expression);
            return expression;
        }

        /// <summary>
        /// Adds the variables.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private Expression AddVariables(Expression expression)
        {
            // add variable assignments up front
            if (this.variables.Count > 0)
            {
                List<Expression> exprs = new List<Expression>();
                for (int i = 0, n = this.variables.Count; i < n; i++)
                {
                    exprs.Add(MakeAssign(this.variables[i], this.initializers[i]));
                }
                exprs.Add(expression);
                Expression sequence = MakeSequence(exprs);  // yields last expression value

                // use invoke/lambda to create variables via parameters in scope
                Expression[] nulls = this.variables.Select(v => Expression.Constant(null, v.Type)).ToArray();
                expression = Expression.Invoke(Expression.Lambda(sequence, this.variables.ToArray()), nulls);
            }

            return expression;
        }

        /// <summary>
        /// Makes the sequence.
        /// </summary>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        private static Expression MakeSequence(IList<Expression> expressions)
        {
            Expression last = expressions[expressions.Count - 1];
            expressions = expressions.Select(e => e.Type.IsValueType ? Expression.Convert(e, typeof(object)) : e).ToList();
            return Expression.Convert(Expression.Call(typeof(ExecutionBuilder), "Sequence", null, Expression.NewArrayInit(typeof(object), expressions)), last.Type);
        }

        /// <summary>
        /// Sequences the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object Sequence(params object[] values)
        {
            return values[values.Length - 1];
        }

        /// <summary>
        /// Batches the specified items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="stream">if set to <c>true</c> [stream].</param>
        /// <returns></returns>
        public static IEnumerable<R> Batch<T, R>(IEnumerable<T> items, Func<T, R> selector, bool stream)
        {
            var result = items.Select(selector);
            if (!stream)
            {
                return result.ToList();
            }
            else
            {
                return new EnumerateOnce<R>(result);
            }
        }

        /// <summary>
        /// Makes the assign.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static Expression MakeAssign(ParameterExpression variable, Expression value)
        {
            return Expression.Call(typeof(ExecutionBuilder), "Assign", new Type[] { variable.Type }, variable, value);
        }

        /// <summary>
        /// Assigns the specified variable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variable">The variable.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T Assign<T>(ref T variable, T value)
        {
            variable = value;
            return value;
        }

        /// <summary>
        /// Builds the inner.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private Expression BuildInner(Expression expression)
        {
            var eb = new ExecutionBuilder(this.linguist, this.policy, this.executor);
            eb.scope = this.scope;
            eb.receivingMember = this.receivingMember;
            eb.nReaders = this.nReaders;
            eb.nLookup = this.nLookup;
            eb.variableMap = this.variableMap;
            return eb.Build(expression);
        }

        /// <summary>
        /// Visits the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        protected override MemberBinding VisitBinding(MemberBinding binding)
        {
            var save = this.receivingMember;
            this.receivingMember = binding.Member;
            var result = base.VisitBinding(binding);
            this.receivingMember = save;
            return result;
        }

        int nLookup = 0;

        private Expression MakeJoinKey(IList<Expression> key)
        {
            if (key.Count == 1)
            {
                return key[0];
            }
            else
            {
                return Expression.New(typeof(CompoundKey).GetConstructors()[0], Expression.NewArrayInit(typeof(object), key.Select(k => (Expression)Expression.Convert(k, typeof(object)))));
            }
        }

        /// <summary>
        /// Visits the client join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns></returns>
        protected override Expression VisitClientJoin(ClientJoinExpression join)
        {
            // convert client join into a up-front lookup table builder & replace client-join in tree with lookup accessor

            // 1) lookup = query.Select(e => new KVP(key: inner, value: e)).ToLookup(kvp => kvp.Key, kvp => kvp.Value)
            Expression innerKey = MakeJoinKey(join.InnerKey);
            Expression outerKey = MakeJoinKey(join.OuterKey);

            ConstructorInfo kvpConstructor = typeof(KeyValuePair<,>).MakeGenericType(innerKey.Type, join.Projection.Projector.Type).GetConstructor(new Type[] { innerKey.Type, join.Projection.Projector.Type });
            Expression constructKVPair = Expression.New(kvpConstructor, innerKey, join.Projection.Projector);
            ProjectionExpression newProjection = new ProjectionExpression(join.Projection.Select, constructKVPair);

            int iLookup = ++nLookup;
            Expression execution = this.ExecuteProjection(newProjection, false);

            ParameterExpression kvp = Expression.Parameter(constructKVPair.Type, "kvp");

            // filter out nulls
            if (join.Projection.Projector.NodeType == (ExpressionType)DbExpressionType.OuterJoined)
            {
                LambdaExpression pred = Expression.Lambda(Expression.PropertyOrField(kvp, "Value").NotEqual(TypeHelper.GetNullConstant(join.Projection.Projector.Type)), kvp);
                execution = Expression.Call(typeof(Enumerable), "Where", new Type[] { kvp.Type }, execution, pred);
            }

            // make lookup
            LambdaExpression keySelector = Expression.Lambda(Expression.PropertyOrField(kvp, "Key"), kvp);
            LambdaExpression elementSelector = Expression.Lambda(Expression.PropertyOrField(kvp, "Value"), kvp);
            Expression toLookup = Expression.Call(typeof(Enumerable), "ToLookup", new Type[] { kvp.Type, outerKey.Type, join.Projection.Projector.Type }, execution, keySelector, elementSelector);

            // 2) agg(lookup[outer])
            ParameterExpression lookup = Expression.Parameter(toLookup.Type, "lookup" + iLookup);
            PropertyInfo property = lookup.Type.GetProperty("Item");
            Expression access = Expression.Call(lookup, property.GetGetMethod(), this.Visit(outerKey));
            if (join.Projection.Aggregator != null)
            {
                // apply aggregator
                access = DbExpressionReplacer.Replace(join.Projection.Aggregator.Body, join.Projection.Aggregator.Parameters[0], access);
            }

            this.variables.Add(lookup);
            this.initializers.Add(toLookup);

            return access;
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression projection)
        {
            if (this.isTop)
            {
                this.isTop = false;
                return this.ExecuteProjection(projection, this.scope != null);
            }
            else
            {
                return this.BuildInner(projection);
            }
        }

        /// <summary>
        /// Parameterizes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected virtual Expression Parameterize(Expression expression)
        {
            if (this.variableMap.Count > 0)
            {
                expression = VariableSubstitutor.Substitute(this.variableMap, expression);
            }
            return this.linguist.Parameterize(expression);
        }

        /// <summary>
        /// Executes the projection.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="okayToDefer">if set to <c>true</c> [okay to defer].</param>
        /// <returns></returns>
        private Expression ExecuteProjection(ProjectionExpression projection, bool okayToDefer)
        {
            // parameterize query
            projection = (ProjectionExpression)this.Parameterize(projection);

            if (this.scope != null)
            {
                // also convert references to outer alias to named values!  these become SQL parameters too
                projection = (ProjectionExpression)OuterParameterizer.Parameterize(this.scope.Alias, projection);
            }

            FixColumnNames(projection);

            string commandText = this.linguist.Format(projection.Select);
            ReadOnlyCollection<NamedValueExpression> namedValues = NamedValueGatherer.Gather(projection.Select);
            QueryCommand command = new QueryCommand(commandText, namedValues.Select(v => new QueryParameter(v.Name, v.Type)));
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            return this.ExecuteProjection(projection, okayToDefer, command, values);
        }

        private void FixColumnNames(ProjectionExpression projection)
        {
            NewExpression newExpress = projection.Projector as NewExpression;

            //need to fix the column names...
            if (newExpress != null)
            {
                List<ColumnExpression> memberColumns = new List<ColumnExpression>();
                foreach(var v in newExpress.Arguments)
                {
                    ColumnExpression cExpress = v as ColumnExpression;
                    if(cExpress != null)
                        memberColumns.Add(cExpress);
                }

                if(memberColumns.Count() == projection.Select.Columns.Count)
                {
                    for (int i = 0; i < projection.Select.Columns.Count; i++)
                    {
                        ColumnDeclaration dec = projection.Select.Columns[i];
                        ColumnExpression match = memberColumns.FirstOrDefault(r => r.Name.Equals(dec.Name));
                        MemberInfo mi = newExpress.Members[memberColumns.IndexOf(match)];
                        dec.Name = linguist.Translator.Mapper.Mapping.GetColumnName(mi);
                        match.Name = dec.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the projection.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="okayToDefer">if set to <c>true</c> [okay to defer].</param>
        /// <param name="command">The command.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private Expression ExecuteProjection(ProjectionExpression projection, bool okayToDefer, QueryCommand command, Expression[] values)
        {
            okayToDefer &= (this.receivingMember != null && this.policy.IsDeferLoaded(this.receivingMember));

            var saveScope = this.scope;
            ParameterExpression reader = Expression.Parameter(typeof(FieldReader), "r" + nReaders++);
            this.scope = new Scope(this.scope, reader, projection.Select.Alias, projection.Select.Columns);
            LambdaExpression projector = Expression.Lambda(this.Visit(projection.Projector), reader);
            this.scope = saveScope;

            var entity = EntityFinder.Find(projection.Projector);

            string methExecute = okayToDefer ? "ExecuteDeferred" : "Execute";

            // call low-level execute directly on supplied DbQueryProvider
            Expression result = Expression.Call(this.executor, methExecute, new Type[] { projector.Body.Type }, Expression.Constant(command), projector, Expression.Constant(entity, typeof(MappingEntity)), Expression.NewArrayInit(typeof(object), values));

            if (projection.Aggregator != null)
            {
                // apply aggregator
                result = DbExpressionReplacer.Replace(projection.Aggregator.Body, projection.Aggregator.Parameters[0], result);
            }
            return result;
        }

        protected override Expression VisitBatch(BatchExpression batch)
        {
            if (this.linguist.Language.AllowsMultipleCommands || !IsMultipleCommands(batch.Operation.Body as CommandExpression))
            {
                return this.BuildExecuteBatch(batch);
            }
            else
            {
                var source = this.Visit(batch.Input);
                var op = this.Visit(batch.Operation.Body);
                var fn = Expression.Lambda(op, batch.Operation.Parameters[1]);
                return Expression.Call(this.GetType(), "Batch", new Type[] { TypeHelper.GetElementType(source.Type), batch.Operation.Body.Type }, source, fn, batch.Stream);
            }
        }

        /// <summary>
        /// Builds the execute batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <returns></returns>
        protected virtual Expression BuildExecuteBatch(BatchExpression batch)
        {
            // parameterize query
            Expression operation = this.Parameterize(batch.Operation.Body);

            string commandText = this.linguist.Format(operation);
            var namedValues = NamedValueGatherer.Gather(operation);
            QueryCommand command = new QueryCommand(commandText, namedValues.Select(v => new QueryParameter(v.Name, v.Type)));
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            Expression paramSets = Expression.Call(typeof(Enumerable), "Select", new Type[] { batch.Operation.Parameters[1].Type, typeof(object[]) }, batch.Input, Expression.Lambda(Expression.NewArrayInit(typeof(object), values), new[] { batch.Operation.Parameters[1] }));

            Expression plan = null;

            ProjectionExpression projection = ProjectionFinder.FindProjection(operation);
            if (projection != null)
            {
                var saveScope = this.scope;
                ParameterExpression reader = Expression.Parameter(typeof(FieldReader), "r" + nReaders++);
                this.scope = new Scope(this.scope, reader, projection.Select.Alias, projection.Select.Columns);
                LambdaExpression projector = Expression.Lambda(this.Visit(projection.Projector), reader);
                this.scope = saveScope;

                var entity = EntityFinder.Find(projection.Projector);
                command = new QueryCommand(command.CommandText, command.Parameters);

                plan = Expression.Call(this.executor, "ExecuteBatch", new Type[] { projector.Body.Type },
                        Expression.Constant(command),
                        paramSets,
                        projector,
                        Expression.Constant(entity, typeof(MappingEntity)),
                        batch.BatchSize,
                        batch.Stream
                        );
            }
            else
            {
                plan = Expression.Call(this.executor, "ExecuteBatch", null, Expression.Constant(command), paramSets, batch.BatchSize, batch.Stream);
            }

            return plan;
        }

        /// <summary>
        /// Visits the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected override Expression VisitCommand(CommandExpression command)
        {
            if (this.linguist.Language.AllowsMultipleCommands || !IsMultipleCommands(command))
            {
                return this.BuildExecuteCommand(command);
            }
            else
            {
                return base.VisitCommand(command);
            }
        }

        /// <summary>
        /// Determines whether [is multiple commands] [the specified command].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        ///   <c>true</c> if [is multiple commands] [the specified command]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsMultipleCommands(CommandExpression command)
        {
            if (command == null)
                return false;

            return true;
        }

        /// <summary>
        /// Visits the block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        protected override Expression VisitBlock(BlockCommand block)
        {
            return MakeSequence(this.VisitExpressionList(block.Commands));
        }

        /// <summary>
        /// Visits if.
        /// </summary>
        /// <param name="ifx">The ifx.</param>
        /// <returns></returns>
        protected override Expression VisitIf(IFCommand ifx)
        {
            var test = Expression.Condition(ifx.Check, ifx.IfTrue, ifx.IfFalse != null ? ifx.IfFalse : ifx.IfTrue.Type == typeof(int) ? (Expression)Expression.Property(this.executor, "RowsAffected") : (Expression)Expression.Constant(TypeHelper.GetDefault(ifx.IfTrue.Type), ifx.IfTrue.Type));
            return this.Visit(test);
        }

        /// <summary>
        /// Visits the function.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        protected override Expression VisitFunction(FunctionExpression func)
        {
            if (this.linguist.Language.IsRowsAffectedExpressions(func))
            {
                return Expression.Property(this.executor, "RowsAffected");
            }
            return base.VisitFunction(func);
        }

        /// <summary>
        /// Visits the exists.
        /// </summary>
        /// <param name="exists">The exists.</param>
        /// <returns></returns>
        protected override Expression VisitExists(ExistsExpression exists)
        {
            // how did we get here? Translate exists into count query
           // var colType = this.linguist.Language.TypeSystem.GetColumnType(typeof(int));
            var newSelect = exists.Select.SetColumns(
                    new[] { new ColumnDeclaration("value", new AggregateExpression(typeof(int), "Count", null, false)) }
                    );

            var projection =
                    new ProjectionExpression(
                            newSelect,
                            new ColumnExpression(typeof(int), newSelect.Alias, "value"),
                            Aggregator.GetAggregator(typeof(int), typeof(IEnumerable<int>))
                            );

            var expression = projection.GreaterThan(Expression.Constant(0));

            return this.Visit(expression);
        }

        /// <summary>
        /// Visits the declaration.
        /// </summary>
        /// <param name="decl">The decl.</param>
        /// <returns></returns>
        protected override Expression VisitDeclaration(DeclarationCommand decl)
        {
            if (decl.Source != null)
            {
                // make query that returns all these declared values as an object[]
                var projection = new ProjectionExpression(
                        decl.Source,
                        Expression.NewArrayInit(
                                typeof(object),
                                decl.Variables.Select(v => v.Expression.Type.IsValueType
                                        ? Expression.Convert(v.Expression, typeof(object))
                                        : v.Expression).ToArray()
                                ),
                        Aggregator.GetAggregator(typeof(object[]), typeof(IEnumerable<object[]>))
                        );

                // create execution variable to hold the array of declared variables
                var vars = Expression.Parameter(typeof(object[]), "vars");
                this.variables.Add(vars);
                this.initializers.Add(Expression.Constant(null, typeof(object[])));

                // create substitution for each variable (so it will find the variable value in the new vars array)
                for (int i = 0, n = decl.Variables.Count; i < n; i++)
                {
                    var v = decl.Variables[i];
                    NamedValueExpression nv = new NamedValueExpression(
                            v.Name,
                            Expression.Convert(Expression.ArrayIndex(vars, Expression.Constant(i)), v.Expression.Type)
                            );
                    this.variableMap.Add(v.Name, nv);
                }

                // make sure the execution of the select stuffs the results into the new vars array
                return MakeAssign(vars, this.Visit(projection));
            }

            // probably bad if we get here since we must not allow multiple commands
            throw new InvalidOperationException("Declaration query not allowed for this langauge");
        }

        /// <summary>
        /// Builds the execute command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected virtual Expression BuildExecuteCommand(CommandExpression command)
        {
            // parameterize query
            var expression = this.Parameterize(command);

            string commandText = this.linguist.Format(expression);
            ReadOnlyCollection<NamedValueExpression> namedValues = NamedValueGatherer.Gather(expression);
            QueryCommand qc = new QueryCommand(commandText, namedValues.Select(v => new QueryParameter(v.Name, v.Type)));
            Expression[] values = namedValues.Select(v => Expression.Convert(this.Visit(v.Value), typeof(object))).ToArray();

            ProjectionExpression projection = ProjectionFinder.FindProjection(expression);
            if (projection != null)
            {
                return this.ExecuteProjection(projection, false, qc, values);
            }

            Expression plan = Expression.Call(this.executor, "ExecuteCommand", null,
                    Expression.Constant(qc),
                    Expression.NewArrayInit(typeof(object), values)
                    );

            return plan;
        }

        protected override Expression VisitEntity(EntityExpression entity)
        {
            return this.Visit(entity.Expression);
        }

        /// <summary>
        /// Visits the outer joined.
        /// </summary>
        /// <param name="outer">The outer.</param>
        /// <returns></returns>
        protected override Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            Expression expr = this.Visit(outer.Expression);
            ColumnExpression column = (ColumnExpression)outer.Test;
            ParameterExpression reader;
            int iOrdinal;
            if (this.scope.TryGetValue(column, out reader, out iOrdinal))
            {
                return Expression.Condition(
                        Expression.Call(reader, "IsDbNull", null, Expression.Constant(iOrdinal)),
                        Expression.Constant(TypeHelper.GetDefault(outer.Type), outer.Type),
                        expr
                        );
            }
            return expr;
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            ParameterExpression fieldReader;
            int iOrdinal;
            if (this.scope != null && this.scope.TryGetValue(column, out fieldReader, out iOrdinal))
            {
                MethodInfo method = FieldReader.GetReaderMethod(column.Type);
                return Expression.Call(fieldReader, method, Expression.Constant(iOrdinal));
            }
            else
            {
                System.Diagnostics.Debug.Fail(string.Format("column not in scope: {0}", column));
            }
            return column;
        }
    }
}