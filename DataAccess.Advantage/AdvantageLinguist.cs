using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Translation;
using DataAccess.Core.Linq.Common;

#pragma warning disable 1591

namespace DataAccess.Advantage
{
    public class AdvantageLinguist : QueryLinguist
    {
        public AdvantageLinguist(AdvantageLanguage language, QueryTranslator translator)
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
            return AdvantageFormatter.Format(expression);
        }
    }
}
