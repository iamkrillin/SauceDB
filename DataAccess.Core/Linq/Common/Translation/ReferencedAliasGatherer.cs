// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#pragma warning disable 1591

using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    ///  returns the set of all aliases produced by a query source
    /// </summary>
    public class ReferencedAliasGatherer : DbExpressionVisitor
    {
        HashSet<TableAlias> aliases;

        private ReferencedAliasGatherer()
        {
            this.aliases = new HashSet<TableAlias>();
        }

        public static HashSet<TableAlias> Gather(Expression source)
        {
            var gatherer = new ReferencedAliasGatherer();
            gatherer.Visit(source);
            return gatherer.aliases;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            this.aliases.Add(column.Alias);
            return column;
        }
    }
}
