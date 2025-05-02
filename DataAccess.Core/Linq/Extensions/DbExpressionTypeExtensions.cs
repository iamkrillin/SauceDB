using System.Linq.Expressions;

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbExpressionTypeExtensions
    {
        /// <summary>
        /// Determines whether [is db expression] [the specified et].
        /// </summary>
        /// <param name="et">The et.</param>
        /// <returns>
        ///   <c>true</c> if [is db expression] [the specified et]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDbExpression(this ExpressionType et)
        {
            return ((int)et) >= 1000;
        }
    }
}
