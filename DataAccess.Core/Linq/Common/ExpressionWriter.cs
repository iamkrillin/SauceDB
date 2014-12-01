// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.IO;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Writes out an expression tree in a C#-ish syntax
    /// </summary>
    public class ExpressionWriter : ExpressionVisitor
    {
        TextWriter writer;
        int indent = 2;
        int depth;
        private static readonly char[] special = new char[] { '\n', '\n', '\\' };
        private static readonly char[] splitters = new char[] { '\n', '\r' };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionWriter"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected ExpressionWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="expression">The expression.</param>
        public static void Write(TextWriter writer, Expression expression)
        {
            new ExpressionWriter(writer).Visit(expression);
        }

        /// <summary>
        /// Writes to string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string WriteToString(Expression expression)
        {
            StringWriter sw = new StringWriter();
            Write(sw, expression);
            return sw.ToString();
        }

        /// <summary>
        /// Gets or sets the width of the indentation.
        /// </summary>
        /// <value>
        /// The width of the indentation.
        /// </value>
        protected int IndentationWidth
        {
            get { return this.indent; }
            set { this.indent = value; }
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="style">The style.</param>
        protected void WriteLine(Indentation style)
        {
            this.writer.WriteLine();
            this.Indent(style);
            for (int i = 0, n = this.depth * this.indent; i < n; i++)
            {
                this.writer.Write(" ");
            }
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        protected void Write(string text)
        {
            if (text.IndexOf('\n') >= 0)
            {
                string[] lines = text.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0, n = lines.Length; i < n; i++)
                {
                    this.Write(lines[i]);
                    if (i < n - 1)
                    {
                        this.WriteLine(Indentation.Same);
                    }
                }
            }
            else
            {
                this.writer.Write(text);
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
        /// Gets the operator.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected virtual string GetOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Not:
                    return "!";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
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
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.RightShift:
                    return ">>";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.ExclusiveOr:
                    return "^";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Visits the binary.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    this.Visit(b.Left);
                    this.Write("[");
                    this.Visit(b.Right);
                    this.Write("]");
                    break;
                case ExpressionType.Power:
                    this.Write("POW(");
                    this.Visit(b.Left);
                    this.Write(", ");
                    this.Visit(b.Right);
                    this.Write(")");
                    break;
                default:
                    this.Visit(b.Left);
                    this.Write(" ");
                    this.Write(GetOperator(b.NodeType));
                    this.Write(" ");
                    this.Visit(b.Right);
                    break;
            }
            return b;
        }

        /// <summary>
        /// Visits the unary.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    this.Write("((");
                    this.Write(this.GetTypeName(u.Type));
                    this.Write(")");
                    this.Visit(u.Operand);
                    this.Write(")");
                    break;
                case ExpressionType.ArrayLength:
                    this.Visit(u.Operand);
                    this.Write(".Length");
                    break;
                case ExpressionType.Quote:
                    this.Visit(u.Operand);
                    break;
                case ExpressionType.TypeAs:
                    this.Visit(u.Operand);
                    this.Write(" as ");
                    this.Write(this.GetTypeName(u.Type));
                    break;
                case ExpressionType.UnaryPlus:
                    this.Visit(u.Operand);
                    break;
                default:
                    this.Write(this.GetOperator(u.NodeType));
                    this.Visit(u.Operand);
                    break;
            }
            return u;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected virtual string GetTypeName(Type type)
        {
            string name = type.Name;
            name = name.Replace('+', '.');
            int iGeneneric = name.IndexOf('`');
            if (iGeneneric > 0)
            {
                name = name.Substring(0, iGeneneric);
            }
            if (type.IsGenericType || type.IsGenericTypeDefinition)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(name);
                sb.Append("<");
                var args = type.GetGenericArguments();
                for (int i = 0, n = args.Length; i < n; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                    if (type.IsGenericType)
                    {
                        sb.Append(this.GetTypeName(args[i]));
                    }
                }
                sb.Append(">");
                name = sb.ToString();
            }
            return name;
        }

        /// <summary>
        /// Visits the conditional.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConditional(ConditionalExpression c)
        {
            this.Visit(c.Test);
            this.WriteLine(Indentation.Inner);
            this.Write("? ");
            this.Visit(c.IfTrue);
            this.WriteLine(Indentation.Same);
            this.Write(": ");
            this.Visit(c.IfFalse);
            this.Indent(Indentation.Outer);
            return c;
        }

        /// <summary>
        /// Visits the binding list.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected override IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.VisitBinding(original[i]);
                if (i < n - 1)
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
            }
            return original;
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value == null)
            {
                this.Write("null");
            }
            else if (c.Type == typeof(string))
            {
                string value = c.Value.ToString();
                if (value.IndexOfAny(special) >= 0)
                    this.Write("@");
                this.Write("\"");
                this.Write(c.Value.ToString());
                this.Write("\"");
            }
            else if (c.Type == typeof(DateTime))
            {
                this.Write("new DateTime(\"");
                this.Write(c.Value.ToString());
                this.Write("\")");
            }
            else if (c.Type.IsArray)
            {
                Type elementType = c.Type.GetElementType();
                this.VisitNewArray(Expression.NewArrayInit(elementType, ((IEnumerable)c.Value).OfType<object>().Select(v => (Expression)Expression.Constant(v, elementType))));
            }
            else
            {
                this.Write(c.Value.ToString());
            }
            return c;
        }

        /// <summary>
        /// Visits the element initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <returns></returns>
        protected override ElementInit VisitElementInitializer(ElementInit initializer)
        {
            if (initializer.Arguments.Count > 1)
            {
                this.Write("{");
                for (int i = 0, n = initializer.Arguments.Count; i < n; i++)
                {
                    this.Visit(initializer.Arguments[i]);
                    if (i < n - 1)
                    {
                        this.Write(", ");
                    }
                }
                this.Write("}");
            }
            else
            {
                this.Visit(initializer.Arguments[0]);
            }
            return initializer;
        }

        /// <summary>
        /// Visits the element initializer list.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected override IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.VisitElementInitializer(original[i]);
                if (i < n - 1)
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
            }
            return original;
        }

        /// <summary>
        /// Visits the expression list.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            for (int i = 0, n = original.Count; i < n; i++)
            {
                this.Visit(original[i]);
                if (i < n - 1)
                {
                    this.Write(",");
                    this.WriteLine(Indentation.Same);
                }
            }
            return original;
        }

        /// <summary>
        /// Visits the invocation.
        /// </summary>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        protected override Expression VisitInvocation(InvocationExpression iv)
        {
            this.Write("Invoke(");
            this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(iv.Arguments);
            this.Write(", ");
            this.WriteLine(Indentation.Same);
            this.Visit(iv.Expression);
            this.WriteLine(Indentation.Same);
            this.Write(")");
            this.Indent(Indentation.Outer);
            return iv;
        }

        /// <summary>
        /// Visits the lambda.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            if (lambda.Parameters.Count != 1)
            {
                this.Write("(");
                for (int i = 0, n = lambda.Parameters.Count; i < n; i++)
                {
                    this.Write(lambda.Parameters[i].Name);
                    if (i < n - 1)
                    {
                        this.Write(", ");
                    }
                }
                this.Write(")");
            }
            else
            {
                this.Write(lambda.Parameters[0].Name);
            }
            this.Write(" => ");
            this.Visit(lambda.Body);
            return lambda;
        }

        /// <summary>
        /// Visits the list init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <returns></returns>
        protected override Expression VisitListInit(ListInitExpression init)
        {
            this.Visit(init.NewExpression);
            this.Write(" {");
            this.WriteLine(Indentation.Inner);
            this.VisitElementInitializerList(init.Initializers);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return init;
        }

        /// <summary>
        /// Visits the member access.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            this.Visit(m.Expression);
            this.Write(".");
            this.Write(m.Member.Name);
            return m;
        }

        /// <summary>
        /// Visits the member assignment.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <returns></returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            this.Write(assignment.Member.Name);
            this.Write(" = ");
            this.Visit(assignment.Expression);
            return assignment;
        }

        /// <summary>
        /// Visits the member init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            this.Visit(init.NewExpression);
            this.Write(" {");
            this.WriteLine(Indentation.Inner);
            this.VisitBindingList(init.Bindings);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return init;
        }

        /// <summary>
        /// Visits the member list binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            this.Write(binding.Member.Name);
            this.Write(" = {");
            this.WriteLine(Indentation.Inner);
            this.VisitElementInitializerList(binding.Initializers);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return binding;
        }

        /// <summary>
        /// Visits the member member binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            this.Write(binding.Member.Name);
            this.Write(" = {");
            this.WriteLine(Indentation.Inner);
            this.VisitBindingList(binding.Bindings);
            this.WriteLine(Indentation.Outer);
            this.Write("}");
            return binding;
        }

        /// <summary>
        /// Visits the method call.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Object != null)
            {
                this.Visit(m.Object);
            }
            else
            {
                this.Write(this.GetTypeName(m.Method.DeclaringType));
            }
            this.Write(".");
            this.Write(m.Method.Name);
            this.Write("(");
            if (m.Arguments.Count > 1)
                this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(m.Arguments);
            if (m.Arguments.Count > 1)
                this.WriteLine(Indentation.Outer);
            this.Write(")");
            return m;
        }

        /// <summary>
        /// Visits the new.
        /// </summary>
        /// <param name="nex">The nex.</param>
        /// <returns></returns>
        protected override NewExpression VisitNew(NewExpression nex)
        {
            this.Write("new ");
            this.Write(this.GetTypeName(nex.Constructor.DeclaringType));
            this.Write("(");
            if (nex.Arguments.Count > 1)
                this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(nex.Arguments);
            if (nex.Arguments.Count > 1)
                this.WriteLine(Indentation.Outer);
            this.Write(")");
            return nex;
        }

        /// <summary>
        /// Visits the new array.
        /// </summary>
        /// <param name="na">The na.</param>
        /// <returns></returns>
        protected override Expression VisitNewArray(NewArrayExpression na)
        {
            this.Write("new ");
            this.Write(this.GetTypeName(TypeHelper.GetElementType(na.Type)));
            this.Write("[] {");
            if (na.Expressions.Count > 1)
                this.WriteLine(Indentation.Inner);
            this.VisitExpressionList(na.Expressions);
            if (na.Expressions.Count > 1)
                this.WriteLine(Indentation.Outer);
            this.Write("}");
            return na;
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            this.Write(p.Name);
            return p;
        }

        /// <summary>
        /// Visits the type is.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            this.Visit(b.Expression);
            this.Write(" is ");
            this.Write(this.GetTypeName(b.TypeOperand));
            return b;
        }

        /// <summary>
        /// Visits the unknown.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected override Expression VisitUnknown(Expression expression)
        {
            this.Write(expression.ToString());
            return expression;
        }
    }
}