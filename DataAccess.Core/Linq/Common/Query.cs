// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using DataAccess.Core.Linq.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// A default implementation of IQueryable for use with QueryProvider
    /// </summary>
    public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
        ///   </returns>
        public Expression Expression { get; private set; }
        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
        ///   </returns>
        public IQueryProvider Provider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public Query(IQueryProvider provider)
            : this(provider, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="staticType">Type of the static.</param>
        public Query(IQueryProvider provider, Type staticType)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("Provider");
            }
            this.Provider = provider;
            this.Expression = staticType != null ? Expression.Constant(this, staticType) : Expression.Constant(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="expression">The expression.</param>
        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("Provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this.Provider = provider;
            this.Expression = expression;
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
        ///   </returns>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute(this.Expression)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Provider.Execute(this.Expression)).GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.Expression.NodeType == ExpressionType.Constant && ((ConstantExpression)this.Expression).Value == this)
            {
                return "Query(" + typeof(T) + ")";
            }
            else
            {
                return this.Expression.ToString();
            }
        }

        /// <summary>
        /// Gets the query text.
        /// </summary>
        public string QueryText
        {
            get
            {
                IQueryText iqt = this.Provider as IQueryText;
                if (iqt != null)
                {
                    return iqt.GetQueryText(this.Expression);
                }
                return "";
            }
        }
    }
}
