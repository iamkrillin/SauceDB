using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Mapping;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityFinder : DbExpressionVisitor
    {
        MappingEntity entity;

        /// <summary>
        /// Finds the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static MappingEntity Find(Expression expression)
        {
            var finder = new EntityFinder();
            finder.Visit(expression);
            return finder.entity;
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (entity == null)
                return base.Visit(exp);
            return exp;
        }

        /// <summary>
        /// Visits the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        protected override Expression VisitEntity(EntityExpression entity)
        {
            if (this.entity == null)
                this.entity = entity.Entity;
            return entity;
        }

        /// <summary>
        /// Visits the new.
        /// </summary>
        /// <param name="nex">The nex.</param>
        /// <returns></returns>
        protected override NewExpression VisitNew(NewExpression nex)
        {
            return nex;
        }

        /// <summary>
        /// Visits the member init.
        /// </summary>
        /// <param name="init">The init.</param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            return init;
        }
    }
}
