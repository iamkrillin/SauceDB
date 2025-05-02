﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    ///  returns the set of all aliases produced by a query source
    /// </summary>
    public class DeclaredAliasGatherer : DbExpressionVisitor
    {
        HashSet<TableAlias> aliases;

        private DeclaredAliasGatherer()
        {
            this.aliases = new HashSet<TableAlias>();
        }

        /// <summary>
        /// Gathers the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static HashSet<TableAlias> Gather(Expression source)
        {
            var gatherer = new DeclaredAliasGatherer();
            gatherer.Visit(source);
            return gatherer.aliases;
        }

        /// <summary>
        /// Visits the select.
        /// </summary>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        protected override Expression VisitSelect(SelectExpression select)
        {
            this.aliases.Add(select.Alias);
            return select;
        }

        /// <summary>
        /// Visits the table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        protected override Expression VisitTable(TableExpression table)
        {
            this.aliases.Add(table.Alias);
            return table;
        }
    }
}
