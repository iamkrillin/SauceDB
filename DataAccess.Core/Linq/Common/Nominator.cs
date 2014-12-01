using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Performs bottom-up analysis to determine which nodes can possibly
    /// be part of an evaluated sub-tree.
    /// </summary>
    public class Nominator : ExpressionVisitor
    {
        Func<Expression, bool> fnCanBeEvaluated;
        HashSet<Expression> candidates;
        bool cannotBeEvaluated;

        private Nominator(Func<Expression, bool> fnCanBeEvaluated)
        {
            this.candidates = new HashSet<Expression>();
            this.fnCanBeEvaluated = fnCanBeEvaluated;
        }

        internal static HashSet<Expression> Nominate(Func<Expression, bool> fnCanBeEvaluated, Expression expression)
        {
            Nominator nominator = new Nominator(fnCanBeEvaluated);
            nominator.Visit(expression);
            return nominator.candidates;
        }

        /// <summary>
        /// Visits the constant.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression c)
        {
            return base.VisitConstant(c);
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
                bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                this.cannotBeEvaluated = false;
                base.Visit(expression);
                if (!this.cannotBeEvaluated)
                {
                    if (this.fnCanBeEvaluated(expression))
                    {
                        this.candidates.Add(expression);
                    }
                    else
                    {
                        this.cannotBeEvaluated = true;
                    }
                }
                this.cannotBeEvaluated |= saveCannotBeEvaluated;
            }
            return expression;
        }
    }
}
