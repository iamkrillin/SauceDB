using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientJoinExpression : DbExpression
    {
        /// <summary>
        /// Gets the outer key.
        /// </summary>
        public ReadOnlyCollection<Expression> OuterKey { get; private set; }

        /// <summary>
        /// Gets the inner key.
        /// </summary>
        public ReadOnlyCollection<Expression> InnerKey { get; private set; }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        public ProjectionExpression Projection { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientJoinExpression"/> class.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="outerKey">The outer key.</param>
        /// <param name="innerKey">The inner key.</param>
        public ClientJoinExpression(ProjectionExpression projection, IEnumerable<Expression> outerKey, IEnumerable<Expression> innerKey)
            : base(DbExpressionType.ClientJoin, projection.Type)
        {
            this.OuterKey = outerKey.ToReadOnly();
            this.InnerKey = innerKey.ToReadOnly();
            this.Projection = projection;
        }
    }
}
