using System;

namespace DataAccess.Core.Linq.Common.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class BasicMappingEntity : MappingEntity
    {
        string _entityID;
        Type _type;

        /// <summary>
        /// Gets the table id.
        /// </summary>
        public override string TableId { get { return _entityID; } }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public override Type ElementType { get { return this._type; } }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public override Type EntityType { get { return this._type; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMappingEntity"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="entityID">The entity ID.</param>
        public BasicMappingEntity(Type type, string entityID)
        {
            this._entityID = entityID;
            this._type = type;
        }
    }
}
