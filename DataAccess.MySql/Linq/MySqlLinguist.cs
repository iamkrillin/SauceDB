using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common.Translation;
using System.Linq.Expressions;

namespace DataAccess.MySql.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlLinguist : QueryLinguist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlLinguist"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="translator">The translator.</param>
        public MySqlLinguist(MySqlLanguage language, QueryTranslator translator)
            : base(language, translator)
        {
        }

        /// <summary>
        /// Provides language specific query translation.  Use this to apply language specific rewrites or
        /// to make assertions/validations about the query.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override Expression Translate(Expression expression)
        {
            // fix up any order-by's
            expression = OrderByRewriter.Rewrite(this.Language, expression);
            expression = base.Translate(expression);
            expression = UnusedColumnRemover.Remove(expression);

            return expression;
        }

        /// <summary>
        /// Converts the query expression into text of this query language
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override string Format(Expression expression)
        {
            return MySqlFormatter.Format(expression);
        }
    }
}
