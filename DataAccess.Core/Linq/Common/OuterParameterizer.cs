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
    class OuterParameterizer : DbExpressionVisitor
    {
        int iParam;
        TableAlias outerAlias;
        Dictionary<ColumnExpression, NamedValueExpression> map = new Dictionary<ColumnExpression, NamedValueExpression>();

        internal static Expression Parameterize(TableAlias outerAlias, Expression expr)
        {
            OuterParameterizer op = new OuterParameterizer();
            op.outerAlias = outerAlias;
            return op.Visit(expr);
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            SelectExpression select = (SelectExpression)this.Visit(proj.Select);
            return this.UpdateProjection(proj, select, proj.Projector, proj.Aggregator);
        }

        /// <summary>
        /// Visits the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override Expression VisitColumn(ColumnExpression column)
        {
            if (column.Alias == this.outerAlias)
            {
                NamedValueExpression nv;
                if (!this.map.TryGetValue(column, out nv))
                {
                    nv = new NamedValueExpression("n" + (iParam++), column);
                    this.map.Add(column, nv);
                }
                return nv;
            }
            return column;
        }
    }
}
