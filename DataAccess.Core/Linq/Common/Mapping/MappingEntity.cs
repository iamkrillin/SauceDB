using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Linq.Common.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class MappingEntity
    {
        /// <summary>
        /// Gets the table id.
        /// </summary>
        public abstract string TableId { get; }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public abstract Type ElementType { get; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public abstract Type EntityType { get; }
    }
}
