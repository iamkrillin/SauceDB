namespace DataAccess.Core.Data
{
    /// <summary>
    /// The type of constraint to append to a query
    /// </summary>
    public enum ConstraintType
    {
        /// <summary>
        /// Will add an "AND" if needed
        /// </summary>
        AND,

        /// <summary>
        /// Will add an "OR" if needed
        /// </summary>
        OR
    }
}
