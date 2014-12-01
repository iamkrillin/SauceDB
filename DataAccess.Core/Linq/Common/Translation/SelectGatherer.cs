// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

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
    /// returns the list of SelectExpressions accessible from the source expression
    /// </summary>
    public class SelectGatherer : DbExpressionVisitor
    {
        List<SelectExpression> selects = new List<SelectExpression>();

        public static ReadOnlyCollection<SelectExpression> Gather(Expression expression)
        {
            var gatherer = new SelectGatherer();
            gatherer.Visit(expression);
            return gatherer.selects.AsReadOnly();
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            this.selects.Add(select);
            return select; // don't visit sub-queries
        }
    }
}