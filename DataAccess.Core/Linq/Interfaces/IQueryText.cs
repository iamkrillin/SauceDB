using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryText
    {
        /// <summary>
        /// Gets the query text.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        string GetQueryText(Expression expression);
    }
}
