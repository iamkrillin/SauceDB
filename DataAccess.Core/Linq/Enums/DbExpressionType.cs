#pragma warning disable 1591

namespace DataAccess.Core.Linq.Enums
{
    /// <summary>
    /// Extended node types for custom expressions
    /// </summary>
    public enum DbExpressionType
    {
        Table = 1000, // make sure these don't overlap with ExpressionType
        ClientJoin,
        Column,
        Select,
        Projection,
        Entity,
        Join,
        Aggregate,
        Scalar,
        Exists,
        In,
        Grouping,
        AggregateSubquery,
        IsNull,
        Between,
        RowCount,
        NamedValue,
        OuterJoined,
        Batch,
        Function,
        Block,
        If,
        Declaration,
        Variable
    }
}
