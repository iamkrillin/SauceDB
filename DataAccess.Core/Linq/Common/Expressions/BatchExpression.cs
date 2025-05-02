using DataAccess.Core.Linq.Enums;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BatchExpression"/> class.
    /// </remarks>
    /// <param name="input">The input.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="batchSize">Size of the batch.</param>
    /// <param name="stream">The stream.</param>
    public class BatchExpression(Expression input, LambdaExpression operation, Expression batchSize, Expression stream) : Expression((ExpressionType)DbExpressionType.Batch, typeof(IEnumerable<>).MakeGenericType(operation.Body.Type))
    {
        /// <summary>
        /// Gets the input.
        /// </summary>
        public Expression Input { get; private set; } = input;

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public LambdaExpression Operation { get; private set; } = operation;

        /// <summary>
        /// Gets the size of the batch.
        /// </summary>
        /// <value>
        /// The size of the batch.
        /// </value>
        public Expression BatchSize { get; private set; } = batchSize;

        /// <summary>
        /// Gets the stream.
        /// </summary>
        public Expression Stream { get; private set; } = stream;
    }
}
