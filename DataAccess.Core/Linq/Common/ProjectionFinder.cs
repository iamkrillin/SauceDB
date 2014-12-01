using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Expressions;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ProjectionFinder : DbExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        ProjectionExpression found = null;

        /// <summary>
        /// Finds the projection.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        internal static ProjectionExpression FindProjection(Expression expression)
        {
            var finder = new ProjectionFinder();
            finder.Visit(expression);
            return finder.found;
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            this.found = proj;
            return proj;
        }
    }
}
