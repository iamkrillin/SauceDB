// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DataAccess.Core.Linq.Common;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Rewrites an expression tree so that locally isolatable sub-expressions are evaluated and converted into ConstantExpression nodes.
    /// </summary>
    public static class PartialEvaluator
    {
        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression Eval(Expression expression)
        {
            return Eval(expression, null, null);
        }

        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return Eval(expression, fnCanBeEvaluated, null);
        }

        /// <summary>
        /// Evals the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="fnCanBeEvaluated">The fn can be evaluated.</param>
        /// <param name="fnPostEval">The fn post eval.</param>
        /// <returns></returns>
        public static Expression Eval(Expression expression, Func<Expression, bool> fnCanBeEvaluated, Func<ConstantExpression, Expression> fnPostEval)
        {
            if (fnCanBeEvaluated == null)
                fnCanBeEvaluated = PartialEvaluator.CanBeEvaluatedLocally;
            return SubtreeEvaluator.Eval(Nominator.Nominate(fnCanBeEvaluated, expression), fnPostEval, expression);
        }

        /// <summary>
        /// Determines whether this instance [can be evaluated locally] the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can be evaluated locally] the specified expression; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }
    }
}