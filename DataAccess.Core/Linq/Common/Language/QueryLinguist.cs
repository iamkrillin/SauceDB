using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Translation;

namespace DataAccess.Core.Linq.Common.Language
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryLinguist
    {
        /// <summary>
        /// Gets the language.
        /// </summary>
        public QueryLanguage Language { get; private set; }

        /// <summary>
        /// Gets the translator.
        /// </summary>
        public QueryTranslator Translator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLinguist"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="translator">The translator.</param>
        public QueryLinguist(QueryLanguage language, QueryTranslator translator)
        {
            this.Language = language;
            this.Translator = translator;
        }

        /// <summary>
        /// Provides language specific query translation.  Use this to apply language specific rewrites or
        /// to make assertions/validations about the query.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Translate(Expression expression)
        {
            // remove redundant layers again before cross apply rewrite
            expression = UnusedColumnRemover.Remove(expression);
            expression = RedundantColumnRemover.Remove(expression);
            expression = RedundantSubqueryRemover.Remove(expression);

            // convert cross-apply and outer-apply joins into inner & left-outer-joins if possible
            var rewritten = CrossApplyRewriter.Rewrite(this.Language, expression);

            // convert cross joins into inner joins
            rewritten = CrossJoinRewriter.Rewrite(rewritten);

            if (rewritten != expression)
            {
                expression = rewritten;
                // do final reduction
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
            }

            return expression;
        }

        /// <summary>
        /// Converts the query expression into text of this query language
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual string Format(Expression expression)
        {
            // use common SQL formatter by default
            return SqlFormatter.Format(expression);
        }

        /// <summary>
        /// Determine which sub-expressions must be parameters
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Parameterize(Expression expression)
        {
            return Parameterizer.Parameterize(this.Language, expression);
        }
    }
}
