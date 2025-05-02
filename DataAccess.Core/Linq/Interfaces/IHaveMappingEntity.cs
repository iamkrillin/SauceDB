using DataAccess.Core.Linq.Common.Mapping;

namespace DataAccess.Core.Linq.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHaveMappingEntity
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        MappingEntity Entity { get; }
    }
}
