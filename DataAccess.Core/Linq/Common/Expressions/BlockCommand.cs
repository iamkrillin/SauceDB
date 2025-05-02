using DataAccess.Core.Linq.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
#pragma warning disable 1591

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class BlockCommand : CommandExpression
    {
        public ReadOnlyCollection<Expression> Commands { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCommand"/> class.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public BlockCommand(IList<Expression> commands)
            : base(DbExpressionType.Block, commands[commands.Count - 1].Type)
        {
            this.Commands = commands.ToReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCommand"/> class.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public BlockCommand(params Expression[] commands)
            : this((IList<Expression>)commands)
        {
        }
    }
}
