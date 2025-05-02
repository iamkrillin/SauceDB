using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableDeclaration
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
        /// Initializes a new instance of the <see cref="VariableDeclaration"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="expression">The expression.</param>
        public VariableDeclaration(string name, Expression expression)
        {
            this.Name = name;
            //this.QueryType = type;
            this.Expression = expression;
        }
    }
}
