using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Enums;
using System.Collections.ObjectModel;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class DeclarationCommand : CommandExpression
    {
        /// <summary>
        /// Gets the variables.
        /// </summary>
        public ReadOnlyCollection<VariableDeclaration> Variables { get; private set; }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public SelectExpression Source { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeclarationCommand"/> class.
        /// </summary>
        /// <param name="variables">The variables.</param>
        /// <param name="source">The source.</param>
        public DeclarationCommand(IEnumerable<VariableDeclaration> variables, SelectExpression source)
            : base(DbExpressionType.Declaration, typeof(void))
        {
            this.Variables = variables.ToReadOnly();
            this.Source = source;
        }
    }
}
