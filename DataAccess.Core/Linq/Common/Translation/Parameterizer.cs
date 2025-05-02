// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Converts user arguments into named-value parameters
    /// </summary>
    public class Parameterizer : DbExpressionVisitor
    {
        int iParam = 0;
        QueryLanguage language;
        Dictionary<TypeAndValue, NamedValueExpression> map = new Dictionary<TypeAndValue, NamedValueExpression>();
        Dictionary<HashedExpression, NamedValueExpression> pmap = new Dictionary<HashedExpression, NamedValueExpression>();

        private Parameterizer(QueryLanguage language)
        {
            this.language = language;
        }

        /// <summary>
        /// Parameterizes the specified language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression Parameterize(QueryLanguage language, Expression expression)
        {
            return new Parameterizer(language).Visit(expression);
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            // don't parameterize the projector or aggregator!
            SelectExpression select = (SelectExpression)this.Visit(proj.Select);
            return this.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
        }

        /// <summary>
        /// Visits the unary.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <returns></returns>
        protected override Expression VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Convert && u.Operand.NodeType == ExpressionType.ArrayIndex)
            {
                var b = (BinaryExpression)u.Operand;
                if (IsConstantOrParameter(b.Left) && IsConstantOrParameter(b.Right))
                {
                    return this.GetNamedValue(u);
                }
            }
            return base.VisitUnary(u);
        }

        private static bool IsConstantOrParameter(Expression e)
        {
            return e != null && e.NodeType == ExpressionType.Constant || e.NodeType == ExpressionType.Parameter;
        }

        /// <summary>
        /// Visits the binary.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            if (left.NodeType == (ExpressionType)DbExpressionType.NamedValue && right.NodeType == (ExpressionType)DbExpressionType.Column)
            {
                NamedValueExpression nv = (NamedValueExpression)left;
                ColumnExpression c = (ColumnExpression)right;
                left = new NamedValueExpression(nv.Name, nv.Value);
            }
            else if (b.Right.NodeType == (ExpressionType)DbExpressionType.NamedValue && b.Left.NodeType == (ExpressionType)DbExpressionType.Column)
            {
                NamedValueExpression nv = (NamedValueExpression)right;
                ColumnExpression c = (ColumnExpression)left;
                right = new NamedValueExpression(nv.Name, nv.Value);
            }
            return this.UpdateBinary(b, left, right, b.Conversion, b.IsLiftedToNull, b.Method);
        }

        /// <summary>
        /// Visits the column assignment.
        /// </summary>
        /// <param name="ca">The ca.</param>
        /// <returns></returns>
        protected override ColumnAssignment VisitColumnAssignment(ColumnAssignment ca)
        {
            ca = base.VisitColumnAssignment(ca);
            Expression expression = ca.Expression;
            NamedValueExpression nv = expression as NamedValueExpression;
            if (nv != null)
            {
                expression = new NamedValueExpression(nv.Name, nv.Value);
            }
            return this.UpdateColumnAssignment(ca, ca.Column, expression);
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value != null && !IsNumeric(c.Value.GetType()))
            {
                NamedValueExpression nv;
                TypeAndValue tv = new TypeAndValue(c.Type, c.Value);
                if (!this.map.TryGetValue(tv, out nv))
                { // re-use same name-value if same type & value
                    string name = "p" + (iParam++);
                    nv = new NamedValueExpression(name, c);
                    this.map.Add(tv, nv);
                }
                return nv;
            }
            return c;
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return this.GetNamedValue(p);
        }

        /// <summary>
        /// Visits the member access.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            m = (MemberExpression)base.VisitMemberAccess(m);
            NamedValueExpression nv = m.Expression as NamedValueExpression;
            if (nv != null)
            {
                Expression x = Expression.MakeMemberAccess(nv.Value, m.Member);
                return GetNamedValue(x);
            }
            return m;
        }

        private Expression GetNamedValue(Expression e)
        {
            NamedValueExpression nv;
            HashedExpression he = new HashedExpression(e);
            if (!this.pmap.TryGetValue(he, out nv))
            {
                string name = "p" + (iParam++);
                nv = new NamedValueExpression(name, e);
                this.pmap.Add(he, nv);
            }
            return nv;
        }

        private bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }


    }
}