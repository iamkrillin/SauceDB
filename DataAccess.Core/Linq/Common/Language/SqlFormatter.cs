// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;

#pragma warning disable 1591

namespace DataAccess.Core.Linq.Common.Language
{
    /// <summary>
    /// Formats a query expression into common SQL language syntax
    /// </summary>
    public class SqlFormatter : DbExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        private StringBuilder sb;

        /// <summary>
        /// 
        /// </summary>
        private int depth;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<TableAlias, string> aliases;

        /// <summary>
        /// Gets or sets a value indicating whether [hide column aliases].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide column aliases]; otherwise, <c>false</c>.
        /// </value>
        protected bool HideColumnAliases { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide table aliases].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide table aliases]; otherwise, <c>false</c>.
        /// </value>
        protected bool HideTableAliases { get; set; }

        /// <summary>
        /// Gets or sets the width of the indentation.
        /// </summary>
        /// <value>
        /// The width of the indentation.
        /// </value>
        public int IndentationWidth { get; set; }

        /// <summary>
        /// Gets the language.
        /// </summary>
        protected virtual QueryLanguage Language { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nested.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is nested; otherwise, <c>false</c>.
        /// </value>
        protected bool IsNested { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlFormatter"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        public SqlFormatter(QueryLanguage language)
        {
            this.Language = language;
            this.sb = new StringBuilder();
            this.aliases = new Dictionary<TableAlias, string>();
        }

        /// <summary>
        /// Formats the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string Format(Expression expression)
        {
            var formatter = new SqlFormatter(null);
            formatter.Visit(expression);
            return formatter.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.sb.ToString();
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void Write(object value)
        {
            this.sb.Append(value);
        }

        /// <summary>
        /// Writes the name of the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        protected virtual void WriteParameterName(string name)
        {
            this.Write("@" + name);
        }

        /// <summary>
        /// Writes the name of the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        protected virtual void WriteVariableName(string name)
        {
            this.WriteParameterName(name);
        }

        /// <summary>
        /// Writes the name of as alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        protected virtual void WriteAsAliasName(string aliasName)
        {
            this.Write("AS ");
            this.WriteAliasName(aliasName);
        }

        /// <summary>
        /// Writes the name of the alias.
        /// </summary>
        /// <param name="aliasName">Name of the alias.</param>
        protected virtual void WriteAliasName(string aliasName)
        {
            this.Write(aliasName);
        }

        /// <summary>
        /// Writes the name of as column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        protected virtual void WriteAsColumnName(string columnName)
        {
            this.Write("AS ");
            this.WriteColumnName(columnName);
        }

        /// <summary>
        /// Writes the name of the column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        protected virtual void WriteColumnName(string columnName)
        {
            string name = (this.Language != null) ? this.Language.Quote(columnName) : columnName;
            this.Write(name);
        }

        /// <summary>
        /// Writes the name of the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        protected virtual void WriteTableName(string tableName)
        {
            string name = (this.Language != null) ? this.Language.Quote(tableName) : tableName;
            this.Write(name);
        }

        protected void WriteLine(Indentation style)
        {
            sb.AppendLine();
            this.Indent(style);
            for (int i = 0, n = this.depth * this.IndentationWidth; i < n; i++)
            {
                this.Write(" ");
            }
        }

        /// <summary>
        /// Indents the specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        protected void Indent(Indentation style)
        {
            if (style == Indentation.Inner)
            {
                this.depth++;
            }
            else if (style == Indentation.Outer)
            {
                this.depth--;
                System.Diagnostics.Debug.Assert(this.depth >= 0);
            }
        }

        /// <summary>
        /// Gets the name of the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        protected virtual string GetAliasName(TableAlias alias)
        {
            string name;
            if (!this.aliases.TryGetValue(alias, out name))
            {
                name = "A" + alias.GetHashCode() + "?";
                this.aliases.Add(alias, name);
            }
            return name;
        }

        /// <summary>
        /// Adds the alias.
        /// </summary>
        /// <param name="alias">The alias.</param>
        protected void AddAlias(TableAlias alias)
        {
            string name;
            if (!this.aliases.TryGetValue(alias, out name))
            {
                name = "t" + this.aliases.Count;
                this.aliases.Add(alias, name);
            }
        }

        /// <summary>
        /// Adds the aliases.
        /// </summary>
        /// <param name="expr">The expr.</param>
        protected virtual void AddAliases(Expression expr)
        {
            AliasedExpression ax = expr as AliasedExpression;
            if (ax != null)
            {
                this.AddAlias(ax.Alias);
            }
            else
            {
                JoinExpression jx = expr as JoinExpression;
                if (jx != null)
                {
                    this.AddAliases(jx.Left);
                    this.AddAliases(jx.Right);
                }
            }
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == null) return null;

            // check for supported node types first 
            // non-supported ones should not be visited (as they would produce bad SQL)
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.UnaryPlus:
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Power:
                case ExpressionType.Conditional:
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess:
                case ExpressionType.Call:
                case ExpressionType.New:
                case (ExpressionType)DbExpressionType.Table:
                case (ExpressionType)DbExpressionType.Column:
                case (ExpressionType)DbExpressionType.Select:
                case (ExpressionType)DbExpressionType.Join:
                case (ExpressionType)DbExpressionType.Aggregate:
                case (ExpressionType)DbExpressionType.Scalar:
                case (ExpressionType)DbExpressionType.Exists:
                case (ExpressionType)DbExpressionType.In:
                case (ExpressionType)DbExpressionType.AggregateSubquery:
                case (ExpressionType)DbExpressionType.IsNull:
                case (ExpressionType)DbExpressionType.Between:
                case (ExpressionType)DbExpressionType.RowCount:
                case (ExpressionType)DbExpressionType.Projection:
                case (ExpressionType)DbExpressionType.NamedValue:
                case (ExpressionType)DbExpressionType.Block:
                case (ExpressionType)DbExpressionType.If:
                case (ExpressionType)DbExpressionType.Declaration:
                case (ExpressionType)DbExpressionType.Variable:
                case (ExpressionType)DbExpressionType.Function:
                    return base.Visit(exp);

                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.ArrayIndex:
                case ExpressionType.TypeIs:
                case ExpressionType.Parameter:
                case ExpressionType.Lambda:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Invoke:
                case ExpressionType.MemberInit:
                case ExpressionType.ListInit:
                default:
                    this.Write(string.Format("?{0}?(", exp.NodeType));
                    base.Visit(exp);
                    this.Write(")");
                    return exp;
            }
        }

        /// <summary>
        /// Visits the member access.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            throw new NotSupportedException(string.Format("The member access '{0}' is not supported", m.Member));
        }

        /// <summary>
        /// Visits the method call.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Decimal))
            {
                switch (m.Method.Name)
                {
                    case "Add":
                    case "Subtract":
                    case "Multiply":
                    case "Divide":
                    case "Remainder":
                        this.Write("(");
                        this.VisitValue(m.Arguments[0]);
                        this.Write(" ");
                        this.Write(GetOperator(m.Method.Name));
                        this.Write(" ");
                        this.VisitValue(m.Arguments[1]);
                        this.Write(")");
                        return m;
                    case "Negate":
                        this.Write("-");
                        this.Visit(m.Arguments[0]);
                        this.Write("");
                        return m;
                    case "Compare":
                        this.Visit(Expression.Condition(
                                Expression.Equal(m.Arguments[0], m.Arguments[1]),
                                Expression.Constant(0),
                                Expression.Condition(
                                        Expression.LessThan(m.Arguments[0], m.Arguments[1]),
                                        Expression.Constant(-1),
                                        Expression.Constant(1)
                                        )));
                        return m;
                }
            }
            else if (m.Method.Name.Equals("ToString", StringComparison.InvariantCultureIgnoreCase) && m.Object.Type == typeof(string))
            {
                return this.Visit(m.Object);  // no op
            }
            else if (m.Method.Name.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
            {
                if (m.Method.IsStatic && m.Method.DeclaringType == typeof(object))
                {
                    this.Write("(");
                    this.Visit(m.Arguments[0]);
                    this.Write(" = ");
                    this.Visit(m.Arguments[1]);
                    this.Write(")");
                    return m;
                }
                else if (!m.Method.IsStatic && m.Arguments.Count == 1 && m.Arguments[0].Type == m.Object.Type)
                {
                    this.Write("(");
                    this.Visit(m.Object);
                    this.Write(" = ");
                    this.Visit(m.Arguments[0]);
                    this.Write(")");
                    return m;
                }
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        /// <summary>
        /// Determines whether the specified type is integer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is integer; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsInteger(Type type)
        {
            return TypeHelper.IsInteger(type);
        }

        /// <summary>
        /// Visits the new.
        /// </summary>
        /// <param name="nex">The nex.</param>
        /// <returns></returns>
        protected override NewExpression VisitNew(NewExpression nex)
        {
            throw new NotSupportedException(string.Format("The constructor for '{0}' is not supported", nex.Constructor.DeclaringType));
        }

        /// <summary>
        /// Visits the unary.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            string op = this.GetOperator(u);
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    if (IsBoolean(u.Operand.Type) || op.Length > 1)
                    {
                        this.Write(op);
                        this.Write(" ");
                        this.VisitPredicate(u.Operand);
                    }
                    else
                    {
                        this.Write(op);
                        this.VisitValue(u.Operand);
                    }
                    break;
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.Write(op);
                    this.VisitValue(u.Operand);
                    break;
                case ExpressionType.UnaryPlus:
                    this.VisitValue(u.Operand);
                    break;
                case ExpressionType.Convert:
                    // ignore conversions for now
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        /// <summary>
        /// Visits the binary.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            string op = this.GetOperator(b);
            Expression left = b.Left;
            Expression right = b.Right;

            this.Write("(");
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (this.IsBoolean(left.Type))
                    {
                        this.VisitPredicate(left);
                        this.Write(" ");
                        this.Write(op);
                        this.Write(" ");
                        this.VisitPredicate(right);
                    }
                    else
                    {
                        this.VisitValue(left);
                        this.Write(" ");
                        this.Write(op);
                        this.Write(" ");
                        this.VisitValue(right);
                    }
                    break;
                case ExpressionType.Equal:
                    if (right.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value == null)
                        {
                            this.Visit(left);
                            this.Write(" IS NULL");
                            break;
                        }
                    }
                    else if (left.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)left;
                        if (ce.Value == null)
                        {
                            this.Visit(right);
                            this.Write(" IS NULL");
                            break;
                        }
                    }
                    goto case ExpressionType.LessThan;
                case ExpressionType.NotEqual:
                    if (right.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value == null)
                        {
                            this.Visit(left);
                            this.Write(" IS NOT NULL");
                            break;
                        }
                    }
                    else if (left.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)left;
                        if (ce.Value == null)
                        {
                            this.Visit(right);
                            this.Write(" IS NOT NULL");
                            break;
                        }
                    }
                    goto case ExpressionType.LessThan;
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    if (left.NodeType == ExpressionType.Call && right.NodeType == ExpressionType.Constant)
                    {
                        MethodCallExpression mc = (MethodCallExpression)left;
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value != null && ce.Value.GetType() == typeof(int) && ((int)ce.Value) == 0)
                        {
                            if (mc.Method.Name.Equals("CompareTo", StringComparison.InvariantCultureIgnoreCase) && !mc.Method.IsStatic && mc.Arguments.Count == 1)
                            {
                                left = mc.Object;
                                right = mc.Arguments[0];
                            }
                            else if ((mc.Method.DeclaringType == typeof(string) || mc.Method.DeclaringType == typeof(decimal)) && mc.Method.Name.Equals("Compare", StringComparison.InvariantCultureIgnoreCase) && mc.Method.IsStatic && mc.Arguments.Count == 2)
                            {
                                left = mc.Arguments[0];
                                right = mc.Arguments[1];
                            }
                        }
                    }
                    goto case ExpressionType.Add;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    this.VisitValue(left);
                    this.Write(" ");
                    this.Write(op);
                    this.Write(" ");
                    this.VisitValue(right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Write(")");
            return b;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        protected virtual string GetOperator(string methodName)
        {
            switch (methodName)
            {
                case "Add": return "+";
                case "Subtract": return "-";
                case "Multiply": return "*";
                case "Divide": return "/";
                case "Negate": return "-";
                case "Remainder": return "%";
                default: return null;
            }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns></returns>
        protected virtual string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return "-";
                case ExpressionType.UnaryPlus:
                    return "+";
                case ExpressionType.Not:
                    return IsBoolean(u.Operand.Type) ? "NOT" : "~";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(b.Left.Type)) ? "AND" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(b.Left.Type) ? "OR" : "|");
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.RightShift:
                    return ">>";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Determines whether the specified type is boolean.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is boolean; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        /// <summary>
        /// Determines whether the specified expr is predicate.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns>
        ///   <c>true</c> if the specified expr is predicate; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(((BinaryExpression)expr).Type);
                case ExpressionType.Not:
                    return IsBoolean(((UnaryExpression)expr).Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case (ExpressionType)DbExpressionType.IsNull:
                case (ExpressionType)DbExpressionType.Between:
                case (ExpressionType)DbExpressionType.Exists:
                case (ExpressionType)DbExpressionType.In:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(((MethodCallExpression)expr).Type);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Visits the predicate.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual Expression VisitPredicate(Expression expr)
        {
            this.Visit(expr);
            if (!IsPredicate(expr))
            {
                this.Write(" <> 0");
            }
            return expr;
        }

        /// <summary>
        /// Visits the value.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        protected virtual Expression VisitValue(Expression expr)
        {
            return this.Visit(expr);
        }

        /// <summary>
        /// Visits the conditional.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConditional(ConditionalExpression c)
        {
            throw new NotSupportedException(string.Format("Conditional expressions not supported"));
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            this.WriteValue(c.Value);
            return c;
        }

        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        protected virtual void WriteValue(object value)
        {
            if (value == null)
            {
                this.Write("NULL");
            }
            else if (value.GetType().IsEnum)
            {
                this.Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
            }
            else
            {
                switch (Type.GetTypeCode(value.GetType()))
                {
                    case TypeCode.Boolean:
                        this.Write(((bool)value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        this.Write("'");
                        this.Write(value);
                        this.Write("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                    case TypeCode.Single:
                    case TypeCode.Double:
                        string str = value.ToString();
                        if (!str.Contains('.'))
                        {
                            str += ".0";
                        }
                        this.Write(str);
                        break;
                    default:
                        this.Write(value);
                        break;
                }
            }
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (column.Alias != null && !this.HideColumnAliases)
            {
                this.WriteAliasName(GetAliasName(column.Alias));
                this.Write(".");
            }
            this.WriteColumnName(column.Name);
            return column;
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            // treat these like scalar subqueries
            if ((proj.Projector is ColumnExpression))
            {
                this.Write("(");
                this.WriteLine(Indentation.Inner);
                this.Visit(proj.Select);
                this.Write(")");
                this.Indent(Indentation.Outer);
            }
            else
            {
                throw new NotSupportedException("Non-scalar projections cannot be translated to SQL.");
            }
            return proj;
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            this.AddAliases(select.From);
            this.Write("SELECT ");
            if (select.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            if (select.Take != null)
            {
                this.WriteTopClause(select.Take);
            }
            this.WriteColumns(select.Columns);
            if (select.From != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("FROM ");
                this.VisitSource(select.From);
            }
            if (select.Where != null)
            {
                this.WriteLine(Indentation.Same);
                this.Write("WHERE ");
                this.VisitPredicate(select.Where);
            }
            if (select.GroupBy != null && select.GroupBy.Count > 0)
            {
                this.WriteLine(Indentation.Same);
                this.Write("GROUP BY ");
                for (int i = 0, n = select.GroupBy.Count; i < n; i++)
                {
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(select.GroupBy[i]);
                }
            }
            if (select.OrderBy != null && select.OrderBy.Count > 0)
            {
                this.WriteLine(Indentation.Same);
                this.Write("ORDER BY ");
                for (int i = 0, n = select.OrderBy.Count; i < n; i++)
                {
                    OrderExpression exp = select.OrderBy[i];
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    this.VisitValue(exp.Expression);
                    if (exp.OrderType != OrderType.Ascending)
                    {
                        this.Write(" DESC");
                    }
                }
            }
            return select;
        }

        /// <summary>
        /// Writes the top clause.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected virtual void WriteTopClause(Expression expression)
        {
            this.Write("TOP (");
            this.Visit(expression);
            this.Write(") ");
        }

        /// <summary>
        /// Writes the columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        protected virtual void WriteColumns(ReadOnlyCollection<ColumnDeclaration> columns)
        {
            if (columns.Count > 0)
            {
                for (int i = 0, n = columns.Count; i < n; i++)
                {
                    ColumnDeclaration column = columns[i];
                    if (i > 0)
                    {
                        this.Write(", ");
                    }
                    ColumnExpression c = this.VisitValue(column.Expression) as ColumnExpression;
                    if (!string.IsNullOrEmpty(column.Name) && (c == null || c.Name != column.Name))
                    {
                        this.Write(" ");
                        this.WriteAsColumnName(column.Name);
                    }
                }
            }
            else
            {
                this.Write("NULL ");
                if (this.IsNested)
                {
                    this.WriteAsColumnName("tmp");
                    this.Write(" ");
                }
            }
        }

        /// <summary>
        /// Visits the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected override Expression VisitSource(Expression source)
        {
            bool saveIsNested = this.IsNested;
            this.IsNested = true;
            switch ((DbExpressionType)source.NodeType)
            {
                case DbExpressionType.Table:
                    TableExpression table = (TableExpression)source;
                    this.WriteTableName(table.Name);
                    if (!this.HideTableAliases)
                    {
                        this.Write(" ");
                        this.WriteAsAliasName(GetAliasName(table.Alias));
                    }
                    break;
                case DbExpressionType.Select:
                    SelectExpression select = (SelectExpression)source;
                    this.Write("(");
                    this.WriteLine(Indentation.Inner);
                    this.Visit(select);
                    this.WriteLine(Indentation.Same);
                    this.Write(") ");
                    this.WriteAsAliasName(GetAliasName(select.Alias));
                    this.Indent(Indentation.Outer);
                    break;
                case DbExpressionType.Join:
                    this.VisitJoin((JoinExpression)source);
                    break;
                default:
                    throw new InvalidOperationException("Select source is not valid type");
            }
            this.IsNested = saveIsNested;
            return source;
        }

        /// <summary>
        /// Visits the join.
        /// </summary>
        /// <param name="join">The join.</param>
        /// <returns></returns>
        protected override Expression VisitJoin(JoinExpression join)
        {
            this.VisitJoinLeft(join.Left);
            this.WriteLine(Indentation.Same);
            switch (join.Join)
            {
                case JoinType.CrossJoin:
                    this.Write("CROSS JOIN ");
                    break;
                case JoinType.InnerJoin:
                    this.Write("INNER JOIN ");
                    break;
                case JoinType.CrossApply:
                    this.Write("CROSS APPLY ");
                    break;
                case JoinType.OuterApply:
                    this.Write("OUTER APPLY ");
                    break;
                case JoinType.LeftOuter:
                case JoinType.SingletonLeftOuter:
                    this.Write("LEFT OUTER JOIN ");
                    break;
            }
            this.VisitJoinRight(join.Right);
            if (join.Condition != null)
            {
                this.WriteLine(Indentation.Inner);
                this.Write("ON ");
                this.VisitPredicate(join.Condition);
                this.Indent(Indentation.Outer);
            }
            return join;
        }

        /// <summary>
        /// Visits the join left.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected virtual Expression VisitJoinLeft(Expression source)
        {
            return this.VisitSource(source);
        }

        /// <summary>
        /// Visits the join right.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected virtual Expression VisitJoinRight(Expression source)
        {
            return this.VisitSource(source);
        }

        /// <summary>
        /// Writes the name of the aggregate.
        /// </summary>
        /// <param name="aggregateName">Name of the aggregate.</param>
        protected virtual void WriteAggregateName(string aggregateName)
        {
            switch (aggregateName)
            {
                case "Average":
                    this.Write("AVG");
                    break;
                case "LongCount":
                    this.Write("COUNT");
                    break;
                default:
                    this.Write(aggregateName.ToUpper());
                    break;
            }
        }

        /// <summary>
        /// Requireses the asterisk when no argument.
        /// </summary>
        /// <param name="aggregateName">Name of the aggregate.</param>
        /// <returns></returns>
        protected virtual bool RequiresAsteriskWhenNoArgument(string aggregateName)
        {
            return aggregateName.Equals("Count", StringComparison.InvariantCultureIgnoreCase) || aggregateName.Equals("LongCount", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Visits the aggregate.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            this.WriteAggregateName(aggregate.AggregateName);
            this.Write("(");
            if (aggregate.IsDistinct)
            {
                this.Write("DISTINCT ");
            }
            if (aggregate.Argument != null)
            {
                this.VisitValue(aggregate.Argument);
            }
            else if (RequiresAsteriskWhenNoArgument(aggregate.AggregateName))
            {
                this.Write("*");
            }
            this.Write(")");
            return aggregate;
        }

        /// <summary>
        /// Visits the is null.
        /// </summary>
        /// <param name="isnull">The isnull.</param>
        /// <returns></returns>
        protected override Expression VisitIsNull(IsNullExpression isnull)
        {
            this.VisitValue(isnull.Expression);
            this.Write(" IS NULL");
            return isnull;
        }

        /// <summary>
        /// Visits the between.
        /// </summary>
        /// <param name="between">The between.</param>
        /// <returns></returns>
        protected override Expression VisitBetween(BetweenExpression between)
        {
            this.VisitValue(between.Expression);
            this.Write(" BETWEEN ");
            this.VisitValue(between.Lower);
            this.Write(" AND ");
            this.VisitValue(between.Upper);
            return between;
        }

        /// <summary>
        /// Visits the row number.
        /// </summary>
        /// <param name="rowNumber">The row number.</param>
        /// <returns></returns>
        protected override Expression VisitRowNumber(RowNumberExpression rowNumber)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Visits the scalar.
        /// </summary>
        /// <param name="subquery">The subquery.</param>
        /// <returns></returns>
        protected override Expression VisitScalar(ScalarExpression subquery)
        {
            this.Write("(");
            this.WriteLine(Indentation.Inner);
            this.Visit(subquery.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return subquery;
        }

        protected override Expression VisitExists(ExistsExpression exists)
        {
            this.Write("EXISTS(");
            this.WriteLine(Indentation.Inner);
            this.Visit(exists.Select);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return exists;
        }

        /// <summary>
        /// Visits the in.
        /// </summary>
        /// <param name="in">The @in.</param>
        /// <returns></returns>
        protected override Expression VisitIn(InExpression @in)
        {
            if (@in.Values != null)
            {
                if (@in.Values.Count == 0)
                {
                    this.Write("0 <> 0");
                }
                else
                {
                    this.VisitValue(@in.Expression);
                    this.Write(" IN (");
                    for (int i = 0, n = @in.Values.Count; i < n; i++)
                    {
                        if (i > 0) this.Write(", ");
                        this.VisitValue(@in.Values[i]);
                    }
                    this.Write(")");
                }
            }
            else
            {
                this.VisitValue(@in.Expression);
                this.Write(" IN (");
                this.WriteLine(Indentation.Inner);
                this.Visit(@in.Select);
                this.WriteLine(Indentation.Same);
                this.Write(")");
                this.Indent(Indentation.Outer);
            }
            return @in;
        }

        /// <summary>
        /// Visits the named value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override Expression VisitNamedValue(NamedValueExpression value)
        {
            this.WriteParameterName(value.Name);
            return value;
        }

        /// <summary>
        /// Visits if.
        /// </summary>
        /// <param name="ifx">The ifx.</param>
        /// <returns></returns>
        protected override Expression VisitIf(IFCommand ifx)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Visits the block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        protected override Expression VisitBlock(BlockCommand block)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Visits the declaration.
        /// </summary>
        /// <param name="decl">The decl.</param>
        /// <returns></returns>
        protected override Expression VisitDeclaration(DeclarationCommand decl)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Visits the variable.
        /// </summary>
        /// <param name="vex">The vex.</param>
        /// <returns></returns>
        protected override Expression VisitVariable(VariableExpression vex)
        {
            this.WriteVariableName(vex.Name);
            return vex;
        }

        /// <summary>
        /// Visits the statement.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected virtual void VisitStatement(Expression expression)
        {
            var p = expression as ProjectionExpression;
            if (p != null)
            {
                this.Visit(p.Select);
            }
            else
            {
                this.Visit(expression);
            }
        }

        /// <summary>
        /// Visits the function.
        /// </summary>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        protected override Expression VisitFunction(FunctionExpression func)
        {
            this.Write(func.Name);
            if (func.Arguments.Count > 0)
            {
                this.Write("(");
                for (int i = 0, n = func.Arguments.Count; i < n; i++)
                {
                    if (i > 0) this.Write(", ");
                    this.Visit(func.Arguments[i]);
                }
                this.Write(")");
            }
            return func;
        }
    }
}
