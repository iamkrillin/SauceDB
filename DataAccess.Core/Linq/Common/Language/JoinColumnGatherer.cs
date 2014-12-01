using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Expressions;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Language
{
    /// <summary>
    /// 
    /// </summary>
    public class JoinColumnGatherer
    {
        /// <summary>
        /// 
        /// </summary>
        HashSet<TableAlias> aliases;

        /// <summary>
        /// 
        /// </summary>
        HashSet<ColumnExpression> columns = new HashSet<ColumnExpression>();

        /// <summary>
        /// Prevents a default instance of the <see cref="JoinColumnGatherer"/> class from being created.
        /// </summary>
        /// <param name="aliases">The aliases.</param>
        private JoinColumnGatherer(HashSet<TableAlias> aliases)
        {
            this.aliases = aliases;
        }

        /// <summary>
        /// Gathers the specified aliases.
        /// </summary>
        /// <param name="aliases">The aliases.</param>
        /// <param name="select">The select.</param>
        /// <returns></returns>
        public static HashSet<ColumnExpression> Gather(HashSet<TableAlias> aliases, SelectExpression select)
        {
            var gatherer = new JoinColumnGatherer(aliases);
            gatherer.Gather(select.Where);
            return gatherer.columns;
        }

        /// <summary>
        /// Gathers the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        private void Gather(Expression expression)
        {
            BinaryExpression b = expression as BinaryExpression;
            if (b != null)
            {
                switch (b.NodeType)
                {
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                        if (IsExternalColumn(b.Left) && GetColumn(b.Right) != null)
                        {
                            this.columns.Add(GetColumn(b.Right));
                        }
                        else if (IsExternalColumn(b.Right) && GetColumn(b.Left) != null)
                        {
                            this.columns.Add(GetColumn(b.Left));
                        }
                        break;
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        if (b.Type == typeof(bool) || b.Type == typeof(bool?))
                        {
                            this.Gather(b.Left);
                            this.Gather(b.Right);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        private ColumnExpression GetColumn(Expression exp)
        {
            while (exp.NodeType == ExpressionType.Convert)
                exp = ((UnaryExpression)exp).Operand;
            return exp as ColumnExpression;
        }

        /// <summary>
        /// Determines whether [is external column] [the specified exp].
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns>
        ///   <c>true</c> if [is external column] [the specified exp]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsExternalColumn(Expression exp)
        {
            var col = GetColumn(exp);
            if (col != null && !this.aliases.Contains(col.Alias))
                return true;
            return false;
        }
    }
}
