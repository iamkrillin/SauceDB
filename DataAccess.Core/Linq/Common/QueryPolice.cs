using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Translation;
using System.Reflection;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryPolice
    {
        /// <summary>
        /// Gets the policy.
        /// </summary>
        public QueryPolicy Policy { get; private set; }
        /// <summary>
        /// Gets the translator.
        /// </summary>
        public QueryTranslator Translator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryPolice"/> class.
        /// </summary>
        /// <param name="policy">The policy.</param>
        /// <param name="translator">The translator.</param>
        public QueryPolice(QueryPolicy policy, QueryTranslator translator)
        {
            this.Policy = policy;
            this.Translator = translator;
        }

        /// <summary>
        /// Applies the policy.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public virtual Expression ApplyPolicy(Expression expression, MemberInfo member)
        {
            return expression;
        }

        /// <summary>
        /// Provides policy specific query translations.  This is where choices about inclusion of related objects and how
        /// heirarchies are materialized affect the definition of the queries.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Translate(Expression expression)
        {
            // add included relationships to client projection
            var rewritten = RelationshipIncluder.Include(this.Translator.Mapper, expression);
            if (rewritten != expression)
            {
                expression = rewritten;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }

            // convert any singleton (1:1 or n:1) projections into server-side joins (cardinality is preserved)
            rewritten = SingletonProjectionRewriter.Rewrite(this.Translator.Linguist.Language, expression);
            if (rewritten != expression)
            {
                expression = rewritten;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }

            // convert projections into client-side joins
            rewritten = ClientJoinedProjectionRewriter.Rewrite(this.Policy, this.Translator.Linguist.Language, expression);
            if (rewritten != expression)
            {
                expression = rewritten;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }

            return expression;
        }

        /// <summary>
        /// Converts a query into an execution plan.  The plan is an function that executes the query and builds the
        /// resulting objects.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public virtual Expression BuildExecutionPlan(Expression query, Expression provider)
        {
            return ExecutionBuilder.Build(this.Translator.Linguist, this.Policy, query, provider);
        }
    }
}
