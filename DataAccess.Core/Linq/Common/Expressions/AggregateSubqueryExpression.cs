using DataAccess.Core.Linq.Enums;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class AggregateSubqueryExpression : DbExpression
    {
        /// <summary>
        /// Gets the group by alias.
        /// </summary>
        public TableAlias GroupByAlias { get; private set; }

        /// <summary>
        /// Gets the aggregate in group select.
        /// </summary>
        public Expression AggregateInGroupSelect { get; private set; }

        /// <summary>
        /// Gets the aggregate as subquery.
        /// </summary>
        public ScalarExpression AggregateAsSubquery { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateSubqueryExpression"/> class.
        /// </summary>
        /// <param name="groupByAlias">The group by alias.</param>
        /// <param name="aggregateInGroupSelect">The aggregate in group select.</param>
        /// <param name="aggregateAsSubquery">The aggregate as subquery.</param>
        public AggregateSubqueryExpression(TableAlias groupByAlias, Expression aggregateInGroupSelect, ScalarExpression aggregateAsSubquery)
            : base(DbExpressionType.AggregateSubquery, aggregateAsSubquery.Type)
        {
            this.AggregateInGroupSelect = aggregateInGroupSelect;
            this.GroupByAlias = groupByAlias;
            this.AggregateAsSubquery = aggregateAsSubquery;
        }
    }
}
