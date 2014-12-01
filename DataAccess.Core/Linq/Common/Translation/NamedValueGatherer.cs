// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class NamedValueGatherer : DbExpressionVisitor
    {
        HashSet<NamedValueExpression> namedValues = new HashSet<NamedValueExpression>(new NamedValueComparer());

        private NamedValueGatherer()
        {
        }

        /// <summary>
        /// Gathers the specified expr.
        /// </summary>
        /// <param name="expr">The expr.</param>
        /// <returns></returns>
        public static ReadOnlyCollection<NamedValueExpression> Gather(Expression expr)
        {
            NamedValueGatherer gatherer = new NamedValueGatherer();
            gatherer.Visit(expr);
            return gatherer.namedValues.ToList().AsReadOnly();
        }

        /// <summary>
        /// Visits the named value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override Expression VisitNamedValue(NamedValueExpression value)
        {
            this.namedValues.Add(value);
            return value;
        }
    }
}