using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    class VariableSubstitutor : DbExpressionVisitor
    {
        Dictionary<string, Expression> map;

        /// <summary>
        /// Prevents a default instance of the <see cref="VariableSubstitutor"/> class from being created.
        /// </summary>
        /// <param name="map">The map.</param>
        private VariableSubstitutor(Dictionary<string, Expression> map)
        {
            this.map = map;
        }

        /// <summary>
        /// Substitutes the specified map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static Expression Substitute(Dictionary<string, Expression> map, Expression expression)
        {
            return new VariableSubstitutor(map).Visit(expression);
        }

        /// <summary>
        /// Visits the variable.
        /// </summary>
        /// <param name="vex">The vex.</param>
        /// <returns></returns>
        protected override Expression VisitVariable(VariableExpression vex)
        {
            Expression sub;
            if (this.map.TryGetValue(vex.Name, out sub))
            {
                return sub;
            }
            return vex;
        }
    }
}
