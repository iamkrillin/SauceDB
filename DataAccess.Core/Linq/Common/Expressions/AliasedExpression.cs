using DataAccess.Core.Linq.Enums;
using System;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AliasedExpression : DbExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AliasedExpression"/> class.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <param name="type">The type.</param>
        /// <param name="alias">The alias.</param>
        protected AliasedExpression(DbExpressionType nodeType, Type type, TableAlias alias)
            : base(nodeType, type)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public TableAlias Alias { get; protected set; }
    }
}
