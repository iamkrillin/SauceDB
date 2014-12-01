using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
    /// </summary>
    class SubtreeEvaluator : ExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        HashSet<Expression> candidates;
        /// <summary>
        /// 
        /// </summary>
        Func<ConstantExpression, Expression> onEval;

        /// <summary>
        /// Prevents a default instance of the <see cref="SubtreeEvaluator"/> class from being created.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="onEval">The on eval.</param>
        private SubtreeEvaluator(HashSet<Expression> candidates, Func<ConstantExpression, Expression> onEval)
        {
            this.candidates = candidates;
            this.onEval = onEval;
        }

        /// <summary>
        /// Evals the specified candidates.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <param name="onEval">The on eval.</param>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        internal static Expression Eval(HashSet<Expression> candidates, Func<ConstantExpression, Expression> onEval, Expression exp)
        {
            return new SubtreeEvaluator(candidates, onEval).Visit(exp);
        }

        /// <summary>
        /// Visits the specified exp.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            if (this.candidates.Contains(exp))
            {
                return this.Evaluate(exp);
            }
            return base.Visit(exp);
        }

        /// <summary>
        /// Posts the eval.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        private Expression PostEval(ConstantExpression e)
        {
            if (this.onEval != null)
            {
                return this.onEval(e);
            }
            return e;
        }

        /// <summary>
        /// Evaluates the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        private Expression Evaluate(Expression e)
        {
            Type type = e.Type;
            if (e.NodeType == ExpressionType.Convert)
            {
                // check for unnecessary convert & strip them
                var u = (UnaryExpression)e;
                if (TypeHelper.GetNonNullableType(u.Operand.Type) == TypeHelper.GetNonNullableType(type))
                {
                    e = ((UnaryExpression)e).Operand;
                }
            }
            if (e.NodeType == ExpressionType.Constant)
            {
                // in case we actually threw out a nullable conversion above, simulate it here
                // don't post-eval nodes that were already constants
                if (e.Type == type)
                {
                    return e;
                }
                else if (TypeHelper.GetNonNullableType(e.Type) == TypeHelper.GetNonNullableType(type))
                {
                    return Expression.Constant(((ConstantExpression)e).Value, type);
                }
            }
            var me = e as MemberExpression;
            if (me != null)
            {
                // member accesses off of constant's are common, and yet since these partial evals
                // are never re-used, using reflection to access the member is faster than compiling  
                // and invoking a lambda
                var ce = me.Expression as ConstantExpression;
                if (ce != null)
                {
                    return this.PostEval(Expression.Constant(me.Member.GetValue(ce.Value), type));
                }
            }
            if (type.IsValueType)
            {
                e = Expression.Convert(e, typeof(object));
            }
            Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(e);
            Func<object> fn = lambda.Compile();
            return this.PostEval(Expression.Constant(fn(), type));
        }
    }
}
