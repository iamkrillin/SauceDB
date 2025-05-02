using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Enums;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// A custom expression representing the construction of one or more result objects from a 
    /// SQL select expression
    /// </summary>
    public class ProjectionExpression : DbExpression
    {
        /// <summary>
        /// Gets the select.
        /// </summary>
        public SelectExpression Select { get; private set; }

        /// <summary>
        /// Gets the projector.
        /// </summary>
        public Expression Projector { get; private set; }

        /// <summary>
        /// Gets the aggregator.
        /// </summary>
        public LambdaExpression Aggregator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionExpression"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="projector">The projector.</param>
        public ProjectionExpression(SelectExpression source, Expression projector)
            : this(source, projector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionExpression"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="projector">The projector.</param>
        /// <param name="aggregator">The aggregator.</param>
        public ProjectionExpression(SelectExpression source, Expression projector, LambdaExpression aggregator)
            : base(DbExpressionType.Projection, aggregator != null ? aggregator.Body.Type : typeof(IEnumerable<>).MakeGenericType(projector.Type))
        {
            this.Select = source;
            this.Projector = projector;
            this.Aggregator = aggregator;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is singleton.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is singleton; otherwise, <c>false</c>.
        /// </value>
        public bool IsSingleton
        {
            get { return this.Aggregator != null && this.Aggregator.Body.Type == Projector.Type; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }

        /// <summary>
        /// Gets the query text.
        /// </summary>
        public string QueryText
        {
            get { return SqlFormatter.Format(Select); }
        }
    }
}
