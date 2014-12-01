// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.IO;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// Writes out an expression tree (including DbExpression nodes) in a C#-ish syntax
    /// </summary>
    public class DbExpressionWriter : ExpressionWriter
    {
        /// <summary>
        /// 
        /// </summary>
        Dictionary<TableAlias, int> aliasMap = new Dictionary<TableAlias, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DbExpressionWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="language">The language.</param>
        protected DbExpressionWriter(TextWriter writer, QueryLanguage language)
            : base(writer)
        {
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="expression">The expression.</param>
        public new static void Write(TextWriter writer, Expression expression)
        {
            Write(writer, null, expression);
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression.</param>
        public static void Write(TextWriter writer, QueryLanguage language, Expression expression)
        {
            new DbExpressionWriter(writer, language).Visit(expression);
        }

        /// <summary>
        /// Writes to string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public new static string WriteToString(Expression expression)
        {
            return WriteToString(null, expression);
        }

        /// <summary>
        /// Writes to string.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string WriteToString(QueryLanguage language, Expression expression)
        {
            StringWriter sw = new StringWriter();
            Write(sw, language, expression);
            return sw.ToString();
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch ((DbExpressionType)exp.NodeType)
            {
                case DbExpressionType.Projection:
                    return this.VisitProjection((ProjectionExpression)exp);
                case DbExpressionType.ClientJoin:
                    return this.VisitClientJoin((ClientJoinExpression)exp);
                case DbExpressionType.Select:
                    return this.VisitSelect((SelectExpression)exp);
                case DbExpressionType.OuterJoined:
                    return this.VisitOuterJoined((OuterJoinedExpression)exp);
                case DbExpressionType.Column:
                    return this.VisitColumn((ColumnExpression)exp);
                case DbExpressionType.If:
                case DbExpressionType.Block:
                case DbExpressionType.Declaration:
                    return this.VisitCommand((CommandExpression)exp);
                case DbExpressionType.Batch:
                    return this.VisitBatch((BatchExpression)exp);
                case DbExpressionType.Function:
                    return this.VisitFunction((FunctionExpression)exp);
                case DbExpressionType.Entity:
                    return this.VisitEntity((EntityExpression)exp);
                default:
                    if (exp is DbExpression)
                    {
                        this.Write(this.FormatQuery(exp));
                        return exp;
                    }
                    else
                    {
                        return base.Visit(exp);
                    }
            }
        }

        /// <summary>
        /// Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        protected void AddAlias(TableAlias alias)
        {
            if (!this.aliasMap.ContainsKey(alias))
            {
                this.aliasMap.Add(alias, this.aliasMap.Count);
            }
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <returns></returns>
        protected virtual Expression VisitProjection(ProjectionExpression projection)
        {
            this.AddAlias(projection.Select.Alias);
            this.Write("Project(");
            this.WriteLine(Indentation.Inner);
            this.Write("@\"");
            this.Visit(projection.Select);
            this.Write("\",");
            this.WriteLine(Indentation.Same);
            this.Visit(projection.Projector);
            this.Write(",");
            this.WriteLine(Indentation.Same);
            this.Visit(projection.Aggregator);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return projection;
        }

        /// <summary>
        /// Visits the client join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns></returns>
        protected virtual Expression VisitClientJoin(ClientJoinExpression join)
        {
            this.AddAlias(join.Projection.Select.Alias);
            this.Write("ClientJoin(");
            this.WriteLine(Indentation.Inner);
            this.Write("OuterKey(");
            this.VisitExpressionList(join.OuterKey);
            this.Write("),");
            this.WriteLine(Indentation.Same);
            this.Write("InnerKey(");
            this.VisitExpressionList(join.InnerKey);
            this.Write("),");
            this.WriteLine(Indentation.Same);
            this.Visit(join.Projection);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return join;
        }

        /// <summary>
        /// Visits the outer joined.
        /// </summary>
        /// <param name="outer">The outer.</param>
        /// <returns></returns>
        protected virtual Expression VisitOuterJoined(OuterJoinedExpression outer)
        {
            this.Write("Outer(");
            this.WriteLine(Indentation.Inner);
            this.Visit(outer.Test);
            this.Write(", ");
            this.WriteLine(Indentation.Same);
            this.Visit(outer.Expression);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return outer;
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected virtual Expression VisitSelect(SelectExpression select)
        {
            this.Write(select.QueryText);
            return select;
        }

        /// <summary>
        /// Visits the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        protected virtual Expression VisitCommand(CommandExpression command)
        {
            this.Write(this.FormatQuery(command));
            return command;
        }

        /// <summary>
        /// Formats the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        protected virtual string FormatQuery(Expression query)
        {
            return SqlFormatter.Format(query);
        }

        /// <summary>
        /// Visits the batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <returns></returns>
        protected virtual Expression VisitBatch(BatchExpression batch)
        {
            this.Write("Batch(");
            this.WriteLine(Indentation.Inner);
            this.Visit(batch.Input);
            this.Write(",");
            this.WriteLine(Indentation.Same);
            this.Visit(batch.Operation);
            this.Write(",");
            this.WriteLine(Indentation.Same);
            this.Visit(batch.BatchSize);
            this.Write(", ");
            this.Visit(batch.Stream);
            this.WriteLine(Indentation.Outer);
            this.Write(")");
            return batch;
        }

        /// <summary>
        /// Visits the variable.
        /// </summary>
        /// <param name="vex">The vex.</param>
        /// <returns></returns>
        protected virtual Expression VisitVariable(VariableExpression vex)
        {
            this.Write(this.FormatQuery(vex));
            return vex;
        }

        /// <summary>
        /// Visits the function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        protected virtual Expression VisitFunction(FunctionExpression function)
        {
            this.Write("FUNCTION ");
            this.Write(function.Name);
            if (function.Arguments.Count > 0)
            {
                this.Write("(");
                this.VisitExpressionList(function.Arguments);
                this.Write(")");
            }
            return function;
        }

        /// <summary>
        /// Visits the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected virtual Expression VisitEntity(EntityExpression entity)
        {
            this.Visit(entity.Expression);
            return entity;
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Type == typeof(QueryCommand))
            {
                QueryCommand qc = (QueryCommand)c.Value;
                this.Write("new QueryCommand {");
                this.WriteLine(Indentation.Inner);
                this.Write("\"" + qc.CommandText + "\"");
                this.Write(",");
                this.WriteLine(Indentation.Same);
                this.Visit(Expression.Constant(qc.Parameters));
                this.Write(")");
                this.WriteLine(Indentation.Outer);
                return c;
            }
            return base.VisitConstant(c);
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected virtual Expression VisitColumn(ColumnExpression column)
        {
            int iAlias;
            string aliasName = this.aliasMap.TryGetValue(column.Alias, out iAlias) ? "A" + iAlias : "A" + (column.Alias != null ? column.Alias.GetHashCode().ToString() : "") + "?";

            this.Write(aliasName);
            this.Write(".");
            this.Write("Column(\"");
            this.Write(column.Name);
            this.Write("\")");
            return column;
        }
    }
}
