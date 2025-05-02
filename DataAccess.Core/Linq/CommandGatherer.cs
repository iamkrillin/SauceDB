using DataAccess.Core.Linq.Common;
using DataAccess.Core.Linq.Common.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
#pragma warning disable 1591

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// linq stuff
    /// </summary>
    public class CommandGatherer : DbExpressionVisitor
    {
        List<QueryCommand> commands = new List<QueryCommand>();
        public CommandGatherer()
        {

        }
        public static ReadOnlyCollection<QueryCommand> Gather(Expression expression)
        {
            var gatherer = new CommandGatherer();
            gatherer.Visit(expression);
            return gatherer.commands.AsReadOnly();
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            QueryCommand qc = c.Value as QueryCommand;
            if (qc != null)
            {
                this.commands.Add(qc);
            }
            return c;
        }
    }
}
