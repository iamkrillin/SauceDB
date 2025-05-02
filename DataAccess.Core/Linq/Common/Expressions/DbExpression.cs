using DataAccess.Core.Linq.Enums;
using System;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DbExpression"/> class.
    /// </remarks>
    /// <param name="eType">Type of the e.</param>
    /// <param name="type">The type.</param>
    public abstract class DbExpression(DbExpressionType eType, Type type) : Expression((ExpressionType)eType, type)
    {
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return DbExpressionWriter.WriteToString(this);
        }
    }
}
