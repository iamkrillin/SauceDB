using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Translation;
#pragma warning disable 1591

namespace DataAccess.Postgre.Linq
{
    public class PostgreLanguage : QueryLanguage
    {
        public override bool AllowsMultipleCommands
        {
            get { return false; }
        }

        public override bool AllowDistinctInAggregates
        {
            get { return true; }
        }

        public override string Quote(string name)
        {
            return name;
        }

        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            return new FunctionExpression(TypeHelper.GetMemberType(member), "LAST_INSERT_ID()", null);
        }

        public override Expression GetRowsAffectedExpression(Expression command)
        {
            return new FunctionExpression(typeof(int), "ROW_COUNT()", null);
        }

        public override bool IsRowsAffectedExpressions(Expression expression)
        {
            FunctionExpression fex = expression as FunctionExpression;
            return fex != null && fex.Name == "ROW_COUNT()";
        }

        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new PostgreLinguist(this, translator);
        }

        public static readonly QueryLanguage Default = new PostgreLanguage();
    }
}
