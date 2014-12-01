using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SubqueryExpression : DbExpression
    {
        /// <summary>
        /// Gets the select.
        /// </summary>
        public SelectExpression Select { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubqueryExpression"/> class.
        /// </summary>
        /// <param name="eType">Type of the e.</param>
        /// <param name="type">The type.</param>
        /// <param name="select">The select.</param>
        protected SubqueryExpression(DbExpressionType eType, Type type, SelectExpression select)
            : base(eType, type)
        {
            System.Diagnostics.Debug.Assert(eType == DbExpressionType.Scalar || eType == DbExpressionType.Exists || eType == DbExpressionType.In);
            this.Select = select;
        }

    }
}
