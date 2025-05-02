// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Replaces references to one specific instance of an expression node with another node
    /// </summary>
    public class ExpressionReplacer : ExpressionVisitor
    {
        Expression searchFor;
        Expression replaceWith;

        private ExpressionReplacer(Expression searchFor, Expression replaceWith)
        {
            this.searchFor = searchFor;
            this.replaceWith = replaceWith;
        }

        /// <summary>
        /// Replaces the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="searchFor">The search for.</param>
        /// <param name="replaceWith">The replace with.</param>
        /// <returns></returns>
        public static Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
        {
            return new ExpressionReplacer(searchFor, replaceWith).Visit(expression);
        }

        /// <summary>
        /// Replaces all.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="searchFor">The search for.</param>
        /// <param name="replaceWith">The replace with.</param>
        /// <returns></returns>
        public static Expression ReplaceAll(Expression expression, Expression[] searchFor, Expression[] replaceWith)
        {
            for (int i = 0, n = searchFor.Length; i < n; i++)
            {
                expression = Replace(expression, searchFor[i], replaceWith[i]);
            }
            return expression;
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == this.searchFor)
            {
                return this.replaceWith;
            }
            return base.Visit(exp);
        }
    }
}
