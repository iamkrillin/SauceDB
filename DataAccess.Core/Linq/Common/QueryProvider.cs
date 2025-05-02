// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using DataAccess.Core.Linq.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// A basic abstract LINQ query provider
    /// </summary>
    public abstract class QueryProvider : IQueryProvider, IQueryText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProvider"/> class.
        /// </summary>
        protected QueryProvider()
        {
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this, expression);
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable"/> object that can evaluate the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable"/> that can evaluate the query represented by the specified expression tree.
        /// </returns>
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeHelper.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Executes the specified expression.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        /// <summary>
        /// Gets the query text.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public abstract string GetQueryText(Expression expression);

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// The value that results from executing the specified query.
        /// </returns>
        public abstract object Execute(Expression expression);
    }
}
