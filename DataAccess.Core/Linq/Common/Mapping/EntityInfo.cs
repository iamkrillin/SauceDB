namespace DataAccess.Core.Linq.Common.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityInfo
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the mapping.
        /// </summary>
        public MappingEntity Mapping { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityInfo"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="mapping">The mapping.</param>
        public EntityInfo(object instance, MappingEntity mapping)
        {
            this.Instance = instance;
            this.Mapping = mapping;
        }
    }
}
