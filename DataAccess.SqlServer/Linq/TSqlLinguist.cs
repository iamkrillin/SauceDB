using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common.Translation;
using System.Linq.Expressions;

namespace DataAccess.SqlServer.Linq
{
    internal class TSqlLinguist : QueryLinguist
    {
        public TSqlLinguist(TSqlLanguage language, QueryTranslator translator)
            : base(language, translator)
        {
        }

        public override Expression Translate(Expression expression)
        {
            // fix up any order-by's
            expression = OrderByRewriter.Rewrite(this.Language, expression);
            expression = base.Translate(expression);

            // convert skip/take info into RowNumber pattern
            expression = SkipToRowNumberRewriter.Rewrite(this.Language, expression);

            // fix up any order-by's we may have changed
            expression = OrderByRewriter.Rewrite(this.Language, expression);

            return expression;
        }

        public override string Format(Expression expression)
        {
            return TSqlFormatter.Format(expression, this.Language);
        }
    }
}
