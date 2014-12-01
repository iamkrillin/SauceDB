using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    public class BatchExpression : Expression
    {
        /// <summary>
        /// Gets the input.
        /// </summary>
        public Expression Input { get; private set; }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public LambdaExpression Operation { get; private set; }

        /// <summary>
        /// Gets the size of the batch.
        /// </summary>
        /// <value>
        /// The size of the batch.
        /// </value>
        public Expression BatchSize { get; private set; }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        public Expression Stream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchExpression"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="stream">The stream.</param>
        public BatchExpression(Expression input, LambdaExpression operation, Expression batchSize, Expression stream)
            : base((ExpressionType)DbExpressionType.Batch, typeof(IEnumerable<>).MakeGenericType(operation.Body.Type))
        {
            this.Input = input;
            this.Operation = operation;
            this.BatchSize = batchSize;
            this.Stream = stream;
        }
    }
}
