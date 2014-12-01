using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DbExpression : Expression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbExpression"/> class.
        /// </summary>
        /// <param name="eType">Type of the e.</param>
        /// <param name="type">The type.</param>
        protected DbExpression(DbExpressionType eType, Type type)
            : base((ExpressionType)eType, type)
        {
        }

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
