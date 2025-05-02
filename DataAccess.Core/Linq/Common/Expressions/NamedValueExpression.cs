using DataAccess.Core.Linq.Enums;
using System;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class NamedValueExpression : DbExpression
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public Expression Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedValueExpression"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="queryType">Type of the query.</param>
        /// <param name="value">The value.</param>
        public NamedValueExpression(string name, Expression value)
            : base(DbExpressionType.NamedValue, value.Type)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            this.Name = name;
            //this.QueryType = queryType;
            this.Value = value;
        }
    }
}
