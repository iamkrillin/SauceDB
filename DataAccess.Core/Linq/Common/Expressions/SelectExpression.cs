using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;
using System.Collections.ObjectModel;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// A custom expression node used to represent a SQL SELECT expression
    /// </summary>
    public class SelectExpression : AliasedExpression
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        public ReadOnlyCollection<ColumnDeclaration> Columns { get; private set; }

        /// <summary>
        /// Gets from.
        /// </summary>
        public Expression From { get; private set; }

        /// <summary>
        /// Gets the where.
        /// </summary>
        public Expression Where { get; private set; }

        /// <summary>
        /// Gets the order by.
        /// </summary>
        public ReadOnlyCollection<OrderExpression> OrderBy { get; private set; }

        /// <summary>
        /// Gets the group by.
        /// </summary>
        public ReadOnlyCollection<Expression> GroupBy { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is distinct.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is distinct; otherwise, <c>false</c>.
        /// </value>
        public bool IsDistinct { get; private set; }

        /// <summary>
        /// Gets the skip.
        /// </summary>
        public Expression Skip { get; private set; }

        /// <summary>
        /// Gets the take.
        /// </summary>
        public Expression Take { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is reverse.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is reverse; otherwise, <c>false</c>.
        /// </value>
        public bool IsReverse { get; private set; }

        /// <summary>
        /// Gets the query text.
        /// </summary>
        public string QueryText { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpression"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="from">From.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="groupBy">The group by.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <param name="skip">The skip.</param>
        /// <param name="take">The take.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        public SelectExpression(TableAlias alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy,
                                                        bool isDistinct, Expression skip, Expression take, bool reverse)
            : base(DbExpressionType.Select, typeof(void), alias)
        {
            this.Columns = columns.ToReadOnly();
            this.IsDistinct = isDistinct;
            this.From = from;
            this.Where = where;
            this.OrderBy = orderBy.ToReadOnly();
            this.GroupBy = groupBy.ToReadOnly();
            this.Take = take;
            this.Skip = skip;
            this.IsReverse = reverse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpression"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="from">From.</param>
        /// <param name="where">The where.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="groupBy">The group by.</param>
        public SelectExpression(TableAlias alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where, IEnumerable<OrderExpression> orderBy, IEnumerable<Expression> groupBy)
            : this(alias, columns, from, where, orderBy, groupBy, false, null, null, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpression"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="from">From.</param>
        /// <param name="where">The where.</param>
        public SelectExpression(TableAlias alias, IEnumerable<ColumnDeclaration> columns, Expression from, Expression where)
            : this(alias, columns, from, where, null, null)
        {
        }
    }
}
