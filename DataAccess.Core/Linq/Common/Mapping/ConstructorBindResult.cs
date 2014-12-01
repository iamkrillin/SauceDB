using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace DataAccess.Core.Linq.Common.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class ConstructorBindResult
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        public NewExpression Expression { get; private set; }

        /// <summary>
        /// Gets the remaining.
        /// </summary>
        public ReadOnlyCollection<EntityAssignment> Remaining { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorBindResult"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="remaining">The remaining.</param>
        public ConstructorBindResult(NewExpression expression, IEnumerable<EntityAssignment> remaining)
        {
            this.Expression = expression;
            this.Remaining = remaining.ToReadOnly();
        }
    }
}
