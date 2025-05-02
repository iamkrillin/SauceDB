using DataAccess.Core.Linq.Common.Mapping;
using DataAccess.Core.Linq.Enums;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityExpression : DbExpression
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        public MappingEntity Entity { get; private set; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityExpression"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="expression">The expression.</param>
        public EntityExpression(MappingEntity entity, Expression expression)
            : base(DbExpressionType.Entity, expression.Type)
        {
            this.Entity = entity;
            this.Expression = expression;
        }
    }
}
