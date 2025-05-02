// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class ComparisonRewriter : DbExpressionVisitor
    {
        SauceMapping mapping;

        private ComparisonRewriter(SauceMapping mapping)
        {
            this.mapping = mapping;
        }

        /// <summary>
        /// Rewrites the specified mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression Rewrite(SauceMapping mapping, Expression expression)
        {
            return new ComparisonRewriter(mapping).Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    Expression result = this.Compare(b);
                    if (result == b)
                        goto default;
                    return this.Visit(result);
                default:
                    return base.VisitBinary(b);
            }
        }

        /// <summary>
        /// Skips the convert.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected Expression SkipConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        /// <summary>
        /// Compares the specified bop.
        /// </summary>
        /// <param name="bop">The bop.</param>
        /// <returns></returns>
        protected Expression Compare(BinaryExpression bop)
        {
            var e1 = this.SkipConvert(bop.Left);
            var e2 = this.SkipConvert(bop.Right);
            EntityExpression entity1 = e1 as EntityExpression;
            EntityExpression entity2 = e2 as EntityExpression;
            bool negate = bop.NodeType == ExpressionType.NotEqual;
            if (entity1 != null)
            {
                return this.MakePredicate(e1, e2, this.mapping.GetPrimaryKeyMembers(entity1.Entity), negate);
            }
            else if (entity2 != null)
            {
                return this.MakePredicate(e1, e2, this.mapping.GetPrimaryKeyMembers(entity2.Entity), negate);
            }
            var dm1 = this.GetDefinedMembers(e1);
            var dm2 = this.GetDefinedMembers(e2);

            if (dm1 == null && dm2 == null)
            {
                // neither are constructed types
                return bop;
            }

            if (dm1 != null && dm2 != null)
            {
                // both are constructed types, so they'd better have the same members declared
                HashSet<string> names1 = new HashSet<string>(dm1.Select(m => m.Name));
                HashSet<string> names2 = new HashSet<string>(dm2.Select(m => m.Name));
                if (names1.IsSubsetOf(names2) && names2.IsSubsetOf(names1))
                {
                    return MakePredicate(e1, e2, dm1, negate);
                }
            }
            else if (dm1 != null)
            {
                return MakePredicate(e1, e2, dm1, negate);
            }
            else if (dm2 != null)
            {
                return MakePredicate(e1, e2, dm2, negate);
            }

            throw new InvalidOperationException("Cannot compare two constructed types with different sets of members assigned.");
        }

        /// <summary>
        /// Makes the predicate.
        /// </summary>
        /// <param name="e1">The e1.</param>
        /// <param name="e2">The e2.</param>
        /// <param name="members">The members.</param>
        /// <param name="negate">if set to <c>true</c> [negate].</param>
        /// <returns></returns>
        protected Expression MakePredicate(Expression e1, Expression e2, IEnumerable<MemberInfo> members, bool negate)
        {
            var pred = members.Select(m => QueryBinder.BindMember(e1, m).Equal(QueryBinder.BindMember(e2, m))).Join(ExpressionType.And);
            if (negate)
                pred = Expression.Not(pred);
            return pred;
        }

        private IEnumerable<MemberInfo> GetDefinedMembers(Expression expr)
        {
            MemberInitExpression mini = expr as MemberInitExpression;
            if (mini != null)
            {
                var members = mini.Bindings.Select(b => FixMember(b.Member));
                if (mini.NewExpression.Members != null)
                {
                    members.Concat(mini.NewExpression.Members.Select(m => FixMember(m)));
                }
                return members;
            }
            else
            {
                NewExpression nex = expr as NewExpression;
                if (nex != null && nex.Members != null)
                {
                    return nex.Members.Select(m => FixMember(m));
                }
            }
            return null;
        }

        private static MemberInfo FixMember(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Method && member.Name.StartsWith("get_"))
            {
                return member.DeclaringType.GetProperty(member.Name.Substring(4));
            }
            return member;
        }
    }
}
