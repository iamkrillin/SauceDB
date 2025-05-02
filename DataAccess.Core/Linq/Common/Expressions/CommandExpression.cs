using DataAccess.Core.Linq.Enums;
using System;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CommandExpression : DbExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExpression"/> class.
        /// </summary>
        /// <param name="eType">Type of the e.</param>
        /// <param name="type">The type.</param>
        protected CommandExpression(DbExpressionType eType, Type type)
            : base(eType, type)
        {
        }
    }
}
