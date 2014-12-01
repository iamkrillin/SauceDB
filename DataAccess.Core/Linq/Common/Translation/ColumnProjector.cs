// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Splits an expression into two parts
    ///   1) a list of column declarations for sub-expressions that must be evaluated on the server
    ///   2) a expression that describes how to combine/project the columns back together into the correct result
    /// </summary>
    public class ColumnProjector : DbExpressionVisitor
    {
        QueryLanguage language;
        Dictionary<ColumnExpression, ColumnExpression> map;
        List<ColumnDeclaration> columns;
        HashSet<string> columnNames;
        HashSet<Expression> candidates;
        HashSet<TableAlias> existingAliases;
        TableAlias newAlias;
        int iColumn;

        private ColumnProjector(QueryLanguage language, Expression expression, IEnumerable<ColumnDeclaration> existingColumns, TableAlias newAlias, IEnumerable<TableAlias> existingAliases)
        {
            this.language = language;
            this.newAlias = newAlias;
            this.existingAliases = new HashSet<TableAlias>(existingAliases);
            this.map = new Dictionary<ColumnExpression, ColumnExpression>();
            if (existingColumns != null)
            {
                this.columns = new List<ColumnDeclaration>(existingColumns);
                this.columnNames = new HashSet<string>(existingColumns.Select(c => c.Name));
            }
            else
            {
                this.columns = new List<ColumnDeclaration>();
                this.columnNames = new HashSet<string>();
            }
            this.candidates = Nominator.Nominate(language, expression);
        }

        /// <summary>
        /// Projects the columns.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="existingColumns">The existing columns.</param>
        /// <param name="newAlias">The new alias.</param>
        /// <param name="existingAliases">The existing aliases.</param>
        /// <returns></returns>
        public static ProjectedColumns ProjectColumns(QueryLanguage language, Expression expression, IEnumerable<ColumnDeclaration> existingColumns, TableAlias newAlias, IEnumerable<TableAlias> existingAliases)
        {
            ColumnProjector projector = new ColumnProjector(language, expression, existingColumns, newAlias, existingAliases);
            Expression expr = projector.Visit(expression);
            return new ProjectedColumns(expr, projector.columns.AsReadOnly());
        }

        /// <summary>
        /// Projects the columns.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="existingColumns">The existing columns.</param>
        /// <param name="newAlias">The new alias.</param>
        /// <param name="existingAliases">The existing aliases.</param>
        /// <returns></returns>
        public static ProjectedColumns ProjectColumns(QueryLanguage language, Expression expression, IEnumerable<ColumnDeclaration> existingColumns, TableAlias newAlias, params TableAlias[] existingAliases)
        {
            return ProjectColumns(language, expression, existingColumns, newAlias, (IEnumerable<TableAlias>)existingAliases);
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression expression)
        {
            if (this.candidates.Contains(expression))
            {
                if (expression.NodeType == (ExpressionType)DbExpressionType.Column)
                {
                    ColumnExpression column = (ColumnExpression)expression;
                    ColumnExpression mapped;
                    if (this.map.TryGetValue(column, out mapped))
                    {
                        return mapped;
                    }
                    // check for column that already refers to this column
                    foreach (ColumnDeclaration existingColumn in this.columns)
                    {
                        ColumnExpression cex = existingColumn.Expression as ColumnExpression;
                        if (cex != null && cex.Alias == column.Alias && cex.Name == column.Name)
                        {
                            // refer to the column already in the column list
                            return new ColumnExpression(column.Type, this.newAlias, existingColumn.Name);
                        }
                    }
                    if (this.existingAliases.Contains(column.Alias))
                    {
                        string columnName = this.GetUniqueColumnName(column.Name);
                        this.columns.Add(new ColumnDeclaration(columnName, column));
                        mapped = new ColumnExpression(column.Type, this.newAlias, columnName);
                        this.map.Add(column, mapped);
                        this.columnNames.Add(columnName);
                        return mapped;
                    }
                    // must be referring to outer scope
                    return column;
                }
                else
                {
                    string columnName = this.GetNextColumnName();
                    this.columns.Add(new ColumnDeclaration(columnName, expression));
                    return new ColumnExpression(expression.Type, this.newAlias, columnName);
                }
            }
            else
            {
                return base.Visit(expression);
            }
        }

        private bool IsColumnNameInUse(string name)
        {
            return this.columnNames.Contains(name);
        }

        private string GetUniqueColumnName(string name)
        {
            string toReturn = name.Substring(0, name.Length - 1); //remove last escape char
            int suffix = 1;
            while (this.IsColumnNameInUse(string.Concat(toReturn, name[name.Length - 1])))
            {
                toReturn = toReturn + "_" +(suffix++);
            }
            return toReturn + name[name.Length - 1]; //add it back
        }

        private string GetNextColumnName()
        {
            return this.GetUniqueColumnName("c" + (iColumn++));
        }
    }
}
