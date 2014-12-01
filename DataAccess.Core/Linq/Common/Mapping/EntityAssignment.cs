using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityAssignment
    {
        /// <summary>
        /// Gets the member.
        /// </summary>
        public MemberInfo Member { get; private set; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAssignment"/> class.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="expression">The expression.</param>
        public EntityAssignment(MemberInfo member, Expression expression)
        {
            this.Member = member;
            this.Expression = expression;
        }
    }
}
