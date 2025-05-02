namespace DataAccess.Core.Linq.Interfaces
{
    /// <summary>
    /// Common interface for controlling defer-loadable types
    /// </summary>
    public interface IDeferLoadable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loaded; otherwise, <c>false</c>.
        /// </value>
        bool IsLoaded { get; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        void Load();
    }
}
