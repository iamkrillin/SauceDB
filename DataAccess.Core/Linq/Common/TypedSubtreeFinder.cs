﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Finds the first sub-expression that is of a specified type
    /// </summary>
    public class TypedSubtreeFinder : ExpressionVisitor
    {
        Expression root;
        Type type;

        private TypedSubtreeFinder(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Finds the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Expression Find(Expression expression, Type type)
        {
            TypedSubtreeFinder finder = new TypedSubtreeFinder(type);
            finder.Visit(expression);
            return finder.root;
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            Expression result = base.Visit(exp);

            // remember the first sub-expression that produces an IQueryable
            if (this.root == null && result != null)
            {
                if (this.type.IsAssignableFrom(result.Type))
                    this.root = result;
            }

            return result;
        }
    }
}