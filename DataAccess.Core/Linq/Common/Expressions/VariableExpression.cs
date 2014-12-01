using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableExpression : Expression
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableExpression"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="queryType">Type of the query.</param>
        public VariableExpression(string name, Type type)
            : base((ExpressionType)DbExpressionType.Variable, type)
        {
            this.Name = name;
        }
    }
}
