using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class ExistsExpression : SubqueryExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExistsExpression"/> class.
        /// </summary>
        /// <param name="select">The select.</param>
        public ExistsExpression(SelectExpression select)
            : base(DbExpressionType.Exists, typeof(bool), select)
        {
        }
    }
}
