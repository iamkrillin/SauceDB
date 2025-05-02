using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common.Translation;
using System.Linq.Expressions;

#pragma warning disable 1591

namespace DataAccess.Postgre.Linq
{
    public class PostgreLinguist : QueryLinguist
    {
        public PostgreLinguist(PostgreLanguage language, QueryTranslator translator)
            : base(language, translator)
        {
        }

        public override Expression Translate(Expression expression)
        {
            // fix up any order-by's
            expression = OrderByRewriter.Rewrite(this.Language, expression);
            expression = base.Translate(expression);
            expression = UnusedColumnRemover.Remove(expression);

            return expression;
        }

        public override string Format(Expression expression)
        {
            return PostgreFormatter.Format(expression);
        }
    }
}
