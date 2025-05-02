namespace DataAccess.Core.Data
{
    /// <summary>
    /// The types of supported foreign key relationships
    /// </summary>
    public enum ForeignKeyType
    {
        /// <summary>
        /// Indicates the data store should take no action
        /// </summary>
        NoAction,

        /// <summary>
        /// Cascading deletes and updates
        /// </summary>
        Cascade,

        /// <summary>
        /// Set null on delete
        /// </summary>
        SetNull
    }
}
