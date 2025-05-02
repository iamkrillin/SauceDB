using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Language;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.MySql.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlLanguage : QueryLanguage
    {
        public MySqlLanguage()
        {
        }

        /// <summary>
        /// Gets a value indicating whether [allows multiple commands].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allows multiple commands]; otherwise, <c>false</c>.
        /// </value>
        public override bool AllowsMultipleCommands
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether [allow distinct in aggregates].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow distinct in aggregates]; otherwise, <c>false</c>.
        /// </value>
        public override bool AllowDistinctInAggregates
        {
            get { return true; }
        }

        /// <summary>
        /// Quotes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override string Quote(string name)
        {
            return name;
        }

        /// <summary>
        /// Gets the generated id expression.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public override Expression GetGeneratedIdExpression(MemberInfo member)
        {
            return new FunctionExpression(TypeHelper.GetMemberType(member), "LAST_INSERT_ID()", null);
        }

        /// <summary>
        /// Gets the rows affected expression.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public override Expression GetRowsAffectedExpression(Expression command)
        {
            return new FunctionExpression(typeof(int), "ROW_COUNT()", null);
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
            return fex != null && fex.Name == "ROW_COUNT()";
        }

        /// <summary>
        /// Creates the linguist.
        /// </summary>
        /// <param name="translator">The translator.</param>
        /// <returns></returns>
        public override QueryLinguist CreateLinguist(QueryTranslator translator)
        {
            return new MySqlLinguist(this, translator);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly QueryLanguage Default = new MySqlLanguage();
    }
}
