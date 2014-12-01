// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ExpressionVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionVisitor"/> class.
        /// </summary>
        protected ExpressionVisitor()
        {
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return this.VisitUnary((UnaryExpression)exp);
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
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Power:
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    return this.VisitUnknown(exp);
            }
        }

        /// <summary>
        /// Visits the unknown.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected virtual Expression VisitUnknown(Expression expression)
        {
            throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
        }

        /// <summary>
        /// Visits the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        /// <summary>
        /// Visits the element initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <returns></returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        /// <summary>
        /// Visits the unary.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns></returns>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            return this.UpdateUnary(u, operand, u.Type, u.Method);
        }

        /// <summary>
        /// Updates the unary.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="operand">The operand.</param>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        protected UnaryExpression UpdateUnary(UnaryExpression u, Expression operand, Type resultType, MethodInfo method)
        {
            if (u.Operand != operand || u.Type != resultType || u.Method != method)
            {
                return Expression.MakeUnary(u.NodeType, operand, resultType, method);
            }
            return u;
        }

        /// <summary>
        /// Visits the binary.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            return this.UpdateBinary(b, left, right, conversion, b.IsLiftedToNull, b.Method);
        }

        /// <summary>
        /// Updates the binary.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="conversion">The conversion.</param>
        /// <param name="isLiftedToNull">if set to <c>true</c> [is lifted to null].</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        protected BinaryExpression UpdateBinary(BinaryExpression b, Expression left, Expression right, Expression conversion, bool isLiftedToNull, MethodInfo method)
        {
            if (left != b.Left || right != b.Right || conversion != b.Conversion || method != b.Method || isLiftedToNull != b.IsLiftedToNull)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                {
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                }
                else
                {
                    return Expression.MakeBinary(b.NodeType, left, right, isLiftedToNull, method);
                }
            }
            return b;
        }

        /// <summary>
        /// Visits the type is.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = this.Visit(b.Expression);
            return this.UpdateTypeIs(b, expr, b.TypeOperand);
        }

        /// <summary>
        /// Updates the type is.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="typeOperand">The type operand.</param>
        /// <returns></returns>
        protected TypeBinaryExpression UpdateTypeIs(TypeBinaryExpression b, Expression expression, Type typeOperand)
        {
            if (expression != b.Expression || typeOperand != b.TypeOperand)
            {
                return Expression.TypeIs(expression, typeOperand);
            }
            return b;
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary>
        /// Visits the conditional.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            return this.UpdateConditional(c, test, ifTrue, ifFalse);
        }

        /// <summary>
        /// Updates the conditional.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="test">The test.</param>
        /// <param name="ifTrue">If true.</param>
        /// <param name="ifFalse">If false.</param>
        /// <returns></returns>
        protected ConditionalExpression UpdateConditional(ConditionalExpression c, Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary>
        /// Visits the member access.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = this.Visit(m.Expression);
            return this.UpdateMemberAccess(m, exp, m.Member);
        }

        /// <summary>
        /// Updates the member access.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        protected MemberExpression UpdateMemberAccess(MemberExpression m, Expression expression, MemberInfo member)
        {
            if (expression != m.Expression || member != m.Member)
            {
                return Expression.MakeMemberAccess(expression, member);
            }
            return m;
        }

        /// <summary>
        /// Visits the method call.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            return this.UpdateMethodCall(m, obj, m.Method, args);
        }

        /// <summary>
        /// Updates the method call.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected MethodCallExpression UpdateMethodCall(MethodCallExpression m, Expression obj, MethodInfo method, IEnumerable<Expression> args)
        {
            if (obj != m.Object || method != m.Method || args != m.Arguments)
            {
                return Expression.Call(obj, method, args);
            }
            return m;
        }

        /// <summary>
        /// Visits the expression list.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            if (original != null)
            {
                List<Expression> list = null;
                for (int i = 0, n = original.Count; i < n; i++)
                {
                    Expression p = this.Visit(original[i]);
                    if (list != null)
                    {
                        list.Add(p);
                    }
                    else if (p != original[i])
                    {
                        list = new List<Expression>(n);
                        for (int j = 0; j < i; j++)
                        {
                            list.Add(original[j]);
                        }
                        list.Add(p);
                    }
                }
                if (list != null)
                {
                    return list.AsReadOnly();
                }
            }
            return original;
        }

        /// <summary>
        /// Visits the member and expression list.
        /// </summary>
        /// <param name="members">The members.</param>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<Expression> VisitMemberAndExpressionList(ReadOnlyCollection<MemberInfo> members, ReadOnlyCollection<Expression> original)
        {
            if (original != null)
            {
                List<Expression> list = null;
                for (int i = 0, n = original.Count; i < n; i++)
                {
                    Expression p = this.VisitMemberAndExpression(members != null ? members[i] : null, original[i]);
                    if (list != null)
                    {
                        list.Add(p);
                    }
                    else if (p != original[i])
                    {
                        list = new List<Expression>(n);
                        for (int j = 0; j < i; j++)
                        {
                            list.Add(original[j]);
                        }
                        list.Add(p);
                    }
                }
                if (list != null)
                {
                    return list.AsReadOnly();
                }
            }
            return original;
        }

        /// <summary>
        /// Visits the member and expression.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected virtual Expression VisitMemberAndExpression(MemberInfo member, Expression expression)
        {
            return this.Visit(expression);
        }

        /// <summary>
        /// Visits the member assignment.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <returns></returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            return this.UpdateMemberAssignment(assignment, assignment.Member, e);
        }

        /// <summary>
        /// Updates the member assignment.
        /// </summary>
        /// <param name="assignment">The assignment.</param>
        /// <param name="member">The member.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected MemberAssignment UpdateMemberAssignment(MemberAssignment assignment, MemberInfo member, Expression expression)
        {
            if (expression != assignment.Expression || member != assignment.Member)
            {



                return Expression.Bind(member, expression);
            }
            return assignment;
        }

        /// <summary>
        /// Visits the member member binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            return this.UpdateMemberMemberBinding(binding, binding.Member, bindings);
        }

        /// <summary>
        /// Updates the member member binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="member">The member.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns></returns>
        protected MemberMemberBinding UpdateMemberMemberBinding(MemberMemberBinding binding, MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            if (bindings != binding.Bindings || member != binding.Member)
            {
                return Expression.MemberBind(member, bindings);
            }
            return binding;
        }

        /// <summary>
        /// Visits the member list binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns></returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            return this.UpdateMemberListBinding(binding, binding.Member, initializers);
        }

        /// <summary>
        /// Updates the member list binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="member">The member.</param>
        /// <param name="initializers">The initializers.</param>
        /// <returns></returns>
        protected MemberListBinding UpdateMemberListBinding(MemberListBinding binding, MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            if (initializers != binding.Initializers || member != binding.Member)
            {
                return Expression.ListBind(member, initializers);
            }
            return binding;
        }

        /// <summary>
        /// Visits the binding list.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        /// <summary>
        /// Visits the element initializer list.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns></returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        /// <summary>
        /// Visits the lambda.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            return this.UpdateLambda(lambda, lambda.Type, body, lambda.Parameters);
        }

        /// <summary>
        /// Updates the lambda.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <param name="delegateType">Type of the delegate.</param>
        /// <param name="body">The body.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected LambdaExpression UpdateLambda(LambdaExpression lambda, Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body != lambda.Body || parameters != lambda.Parameters || delegateType != lambda.Type)
            {
                return Expression.Lambda(delegateType, body, parameters);
            }
            return lambda;
        }

        /// <summary>
        /// Visits the new.
        /// </summary>
        /// <param name="nex">The nex.</param>
        /// <returns></returns>
        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitMemberAndExpressionList(nex.Members, nex.Arguments);
            return this.UpdateNew(nex, nex.Constructor, args, nex.Members);
        }

        /// <summary>
        /// Updates the new.
        /// </summary>
        /// <param name="nex">The nex.</param>
        /// <param name="constructor">The constructor.</param>
        /// <param name="args">The args.</param>
        /// <param name="members">The members.</param>
        /// <returns></returns>
        protected NewExpression UpdateNew(NewExpression nex, ConstructorInfo constructor, IEnumerable<Expression> args, IEnumerable<MemberInfo> members)
        {
            if (args != nex.Arguments || constructor != nex.Constructor || members != nex.Members)
            {
                if (nex.Members != null)
                {
                    return Expression.New(constructor, args, members);
                }
                else
                {
                    return Expression.New(constructor, args);
                }
            }
            return nex;
        }

        /// <summary>
        /// Visits the member init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <returns></returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            return this.UpdateMemberInit(init, n, bindings);
        }

        /// <summary>
        /// Updates the member init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <param name="nex">The nex.</param>
        /// <param name="bindings">The bindings.</param>
        /// <returns></returns>
        protected MemberInitExpression UpdateMemberInit(MemberInitExpression init, NewExpression nex, IEnumerable<MemberBinding> bindings)
        {
            if (nex != init.NewExpression || bindings != init.Bindings)
            {
                return Expression.MemberInit(nex, bindings);
            }
            return init;
        }

        /// <summary>
        /// Visits the list init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <returns></returns>
        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            return this.UpdateListInit(init, n, initializers);
        }

        /// <summary>
        /// Updates the list init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <param name="nex">The nex.</param>
        /// <param name="initializers">The initializers.</param>
        /// <returns></returns>
        protected ListInitExpression UpdateListInit(ListInitExpression init, NewExpression nex, IEnumerable<ElementInit> initializers)
        {
            if (nex != init.NewExpression || initializers != init.Initializers)
            {
                return Expression.ListInit(nex, initializers);
            }
            return init;
        }

        /// <summary>
        /// Visits the new array.
        /// </summary>
        /// <param name="na">The na.</param>
        /// <returns></returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            return this.UpdateNewArray(na, na.Type, exprs);
        }

        /// <summary>
        /// Updates the new array.
        /// </summary>
        /// <param name="na">The na.</param>
        /// <param name="arrayType">Type of the array.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        protected NewArrayExpression UpdateNewArray(NewArrayExpression na, Type arrayType, IEnumerable<Expression> expressions)
        {
            if (expressions != na.Expressions || na.Type != arrayType)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return Expression.NewArrayInit(arrayType.GetElementType(), expressions);
                }
                else
                {
                    return Expression.NewArrayBounds(arrayType.GetElementType(), expressions);
                }
            }
            return na;
        }

        /// <summary>
        /// Visits the invocation.
        /// </summary>
        /// <param name="iv">The iv.</param>
        /// <returns></returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            return this.UpdateInvocation(iv, expr, args);
        }

        /// <summary>
        /// Updates the invocation.
        /// </summary>
        /// <param name="iv">The iv.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        protected InvocationExpression UpdateInvocation(InvocationExpression iv, Expression expression, IEnumerable<Expression> args)
        {
            if (args != iv.Arguments || expression != iv.Expression)
            {
                return Expression.Invoke(expression, args);
            }
            return iv;
        }
    }
}