using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Language;
using DataAccess.Core.Linq.Common;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Expressions;
using System.Reflection;
using DataAccess.Core.Linq;

namespace DataAccess.Xamarin.Android.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class AndroidLanguage : QueryLanguage
    {
        /// <summary>
        /// Gets the generated id expression.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            return new FunctionExpression(TypeHelper.GetMemberType(member), "last_insert_rowid()", null);
        }

        /// <summary>
        /// Gets the rows affected expression.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public override Expression GetRowsAffectedExpression(Expression command)
        {
            return new FunctionExpression(typeof(int), "changes()", null);
        }

        /// <summary>
        /// Determines whether [is rows affected expressions] [the specified expression].
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        ///   <c>true</c> if [is rows affected expressions] [the specified expression]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsRowsAffectedExpressions(Expression expression)
        {
            FunctionExpression fex = expression as FunctionExpression;
            return fex != null && fex.Name == "changes()";
        }

        /// <summary>
        /// Creates the linguist.
        /// </summary>
        /// <param name="translator">The translator.</param>
        /// <returns></returns>
        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new AndroidLinguist(this, translator);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly QueryLanguage Default = new AndroidLanguage();
    }
}
