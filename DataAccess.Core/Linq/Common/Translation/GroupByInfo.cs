using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Translation
{
    /// <summary>
    /// 
    /// </summary>
    public class GroupByInfo
    {
        /// <summary>
        /// Gets the alias.
        /// </summary>
        internal TableAlias Alias { get; private set; }

        /// <summary>
        /// Gets the element.
        /// </summary>
        internal Expression Element { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupByInfo"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="element">The element.</param>
        internal GroupByInfo(TableAlias alias, Expression element)
        {
            this.Alias = alias;
            this.Element = element;
        }
    }
}
