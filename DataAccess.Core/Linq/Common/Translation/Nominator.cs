using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Expressions;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// Nominator is a class that walks an expression tree bottom up, determining the set of 
    /// candidate expressions that are possible columns of a select expression
    /// </summary>
    public class Nominator : DbExpressionVisitor
    {
        QueryLanguage language;
        bool isBlocked;
        HashSet<Expression> candidates;

        private Nominator(QueryLanguage language)
        {
            this.language = language;
            this.candidates = new HashSet<Expression>();
            this.isBlocked = false;
        }

        internal static HashSet<Expression> Nominate(QueryLanguage language, Expression expression)
        {
            Nominator nominator = new Nominator(language);
            nominator.Visit(expression);
            return nominator.candidates;
        }

        /// <summary>
        /// Visits the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression expression)
        {
            if (expression != null)
            {
                bool saveIsBlocked = this.isBlocked;
                this.isBlocked = false;
                if (this.language.MustBeColumn(expression))
                {
                    this.candidates.Add(expression);
                    // don't merge saveIsBlocked
                }
                else
                {
                    base.Visit(expression);
                    if (!this.isBlocked)
                    {
                        if (this.language.CanBeColumn(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.isBlocked = true;
                        }
                    }
                    this.isBlocked |= saveIsBlocked;
                }
            }
            return expression;
        }

        /// <summary>
        /// Visits the projection.
        /// </summary>
        /// <param name="proj">The proj.</param>
        /// <returns></returns>
        protected override Expression VisitProjection(ProjectionExpression proj)
        {
            this.Visit(proj.Projector);
            return proj;
        }
    }
}
