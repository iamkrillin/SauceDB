using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// A declaration of a column in a SQL SELECT expression
    /// </summary>
    public class ColumnDeclaration
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnDeclaration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="queryType">Type of the query.</param>
        public ColumnDeclaration(string name, Expression expression)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (expression == null)
                throw new ArgumentNullException("expression");
            this.Name = name;
            this.Expression = expression;
        }
    }
}
