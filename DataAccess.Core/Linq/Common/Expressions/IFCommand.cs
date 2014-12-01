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
    public class IFCommand : CommandExpression
    {
        /// <summary>
        /// Gets the check.
        /// </summary>
        public Expression Check { get; private set; }

        /// <summary>
        /// Gets if true.
        /// </summary>
        public Expression IfTrue { get; private set; }

        /// <summary>
        /// Gets if false.
        /// </summary>
        public Expression IfFalse { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IFCommand"/> class.
        /// </summary>
        /// <param name="check">The check.</param>
        /// <param name="ifTrue">If true.</param>
        /// <param name="ifFalse">If false.</param>
        public IFCommand(Expression check, Expression ifTrue, Expression ifFalse)
            : base(DbExpressionType.If, ifTrue.Type)
        {
            this.Check = check;
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
        }
    }
}
