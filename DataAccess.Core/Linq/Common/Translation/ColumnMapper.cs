// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Rewrite all column references to one or more aliases to a new single alias
    /// </summary>
    public class ColumnMapper : DbExpressionVisitor
    {
        HashSet<TableAlias> oldAliases;
        TableAlias newAlias;

        private ColumnMapper(IEnumerable<TableAlias> oldAliases, TableAlias newAlias)
        {
            this.oldAliases = new HashSet<TableAlias>(oldAliases);
            this.newAlias = newAlias;
        }

        /// <summary>
        /// Maps the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="newAlias">The new alias.</param>
        /// <param name="oldAliases">The old aliases.</param>
        /// <returns></returns>
        public static Expression Map(Expression expression, TableAlias newAlias, IEnumerable<TableAlias> oldAliases)
        {
            return new ColumnMapper(oldAliases, newAlias).Visit(expression);
        }

        /// <summary>
        /// Maps the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="newAlias">The new alias.</param>
        /// <param name="oldAliases">The old aliases.</param>
        /// <returns></returns>
        public static Expression Map(Expression expression, TableAlias newAlias, params TableAlias[] oldAliases)
        {
            return Map(expression, newAlias, (IEnumerable<TableAlias>)oldAliases);
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (this.oldAliases.Contains(column.Alias))
            {
                return new ColumnExpression(column.Type, this.newAlias, column.Name);
            }
            return column;
        }
    }
}
