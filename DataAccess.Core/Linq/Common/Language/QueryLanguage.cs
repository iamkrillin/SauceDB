// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common.Translation;
using DataAccess.Core.Linq.Enums;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Defines the language rules for the query provider
    /// </summary>
    public abstract class QueryLanguage
    {
        /// <summary>
        /// Gets the type system.
        /// </summary>
        //public abstract QueryTypeSystem TypeSystem { get; }

        /// <summary>
        /// Gets the generated id expression.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public abstract Expression GetGeneratedIdExpression(MemberInfo member);

        /// <summary>
        /// Gets a value indicating whether [allows multiple commands].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allows multiple commands]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowsMultipleCommands { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether [allow subquery in select without from].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow subquery in select without from]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowSubqueryInSelectWithoutFrom { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether [allow distinct in aggregates].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow distinct in aggregates]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowDistinctInAggregates { get { return false; } }

        /// <summary>
        /// Quotes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual string Quote(string name)
        {
            return name;
        }

        /// <summary>
        /// Gets the rows affected expression.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public virtual Expression GetRowsAffectedExpression(Expression command)
        {
            return new FunctionExpression(typeof(int), "@@ROWCOUNT", null);
        }

        /// <summary>
        /// Determines whether [is rows affected expressions] [the specified expression].
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        ///   <c>true</c> if [is rows affected expressions] [the specified expression]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsRowsAffectedExpressions(Expression expression)
        {
            FunctionExpression fex = expression as FunctionExpression;
            return fex != null && fex.Name == "@@ROWCOUNT";
        }

        /// <summary>
        /// Gets the outer join test.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        public virtual Expression GetOuterJoinTest(SelectExpression select)
        {
            var aliases = DeclaredAliasGatherer.Gather(select.From);
            var joinColumns = JoinColumnGatherer.Gather(aliases, select).ToList();
            if (joinColumns.Count > 0)
            {
                // prefer one that is already in the projection list.
                foreach (var jc in joinColumns)
                {
                    foreach (var col in select.Columns)
                    {
                        if (jc.Equals(col.Expression))
                        {
                            return jc;
                        }
                    }
                }
                return joinColumns[0];
            }

            // fall back to introducing a constant
            return Expression.Constant(1, typeof(int?));
        }

        /// <summary>
        /// Adds the outer join test.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        public virtual ProjectionExpression AddOuterJoinTest(ProjectionExpression proj)
        {
            var test = this.GetOuterJoinTest(proj.Select);
            var select = proj.Select;
            ColumnExpression testCol = null;
            // look to see if test expression exists in columns already
            foreach (var col in select.Columns)
            {
                if (test.Equals(col.Expression))
                {
                    testCol = new ColumnExpression(test.Type, select.Alias, col.Name);
                    break;
                }
            }
            if (testCol == null)
            {
                // add expression to projection
                testCol = test as ColumnExpression;
                string colName = (testCol != null) ? testCol.Name : "Test";
                colName = proj.Select.Columns.GetAvailableColumnName(colName);
                select = select.AddColumn(new ColumnDeclaration(colName, test));
                testCol = new ColumnExpression(test.Type, select.Alias, colName);
            }
            var newProjector = new OuterJoinedExpression(testCol, proj.Projector);
            return new ProjectionExpression(select, newProjector, proj.Aggregator);
        }

        /// <summary>
        /// Determines whether the CLR type corresponds to a scalar data type in the query language
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsScalar(Type type)
        {
            type = TypeHelper.GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return false;
                case TypeCode.Object:
                    return
                            type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || type == typeof(Guid) || type == typeof(byte[]);
                default:
                    return true;
            }
        }

        /// <summary>
        /// Determines whether the specified member is aggregate.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if the specified member is aggregate; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsAggregate(MemberInfo member)
        {
            var method = member as MethodInfo;
            if (method != null)
            {
                if (method.DeclaringType == typeof(Queryable)
                        || method.DeclaringType == typeof(Enumerable))
                {
                    switch (method.Name)
                    {
                        case "Count":
                        case "LongCount":
                        case "Sum":
                        case "Min":
                        case "Max":
                        case "Average":
                            return true;
                    }
                }
            }
            var property = member as PropertyInfo;
            if (property != null
                    && property.Name == "Count"
                    && typeof(IEnumerable).IsAssignableFrom(property.DeclaringType))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Aggregates the argument is predicate.
        /// </summary>
        /// <param name="aggregateName">Name of the aggregate.</param>
        /// <returns></returns>
        public virtual bool AggregateArgumentIsPredicate(string aggregateName)
        {
            return aggregateName == "Count" || aggregateName == "LongCount";
        }

        /// <summary>
        /// Determines whether the given expression can be represented as a column in a select expressionss
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can be column] the specified expression; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanBeColumn(Expression expression)
        {
            // by default, push all work in projection to client
            return this.MustBeColumn(expression);
        }

        /// <summary>
        /// Musts the be column.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public virtual bool MustBeColumn(Expression expression)
        {
            switch (expression.NodeType)
            {
                case (ExpressionType)DbExpressionType.Column:
                case (ExpressionType)DbExpressionType.Scalar:
                case (ExpressionType)DbExpressionType.Exists:
                case (ExpressionType)DbExpressionType.AggregateSubquery:
                case (ExpressionType)DbExpressionType.Aggregate:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates the linguist.
        /// </summary>
        /// <param name="translator">The translator.</param>
        /// <returns></returns>
        public virtual QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new QueryLinguist(this, translator);
        }
    }
}