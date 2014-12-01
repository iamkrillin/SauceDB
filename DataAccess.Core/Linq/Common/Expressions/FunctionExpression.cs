using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Enums;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class FunctionExpression : DbExpression
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionExpression"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        public FunctionExpression(Type type, string name, IEnumerable<Expression> arguments)
            : base(DbExpressionType.Function, type)
        {
            this.Name = name;
            this.Arguments = arguments.ToReadOnly();
        }
    }
}
