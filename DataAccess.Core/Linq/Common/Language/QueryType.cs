using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Linq.Common.Language
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class QueryType
    {
        /// <summary>
        /// Gets a value indicating whether [not null].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [not null]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool NotNull { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the precision.
        /// </summary>
        public abstract short Precision { get; }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        public abstract short Scale { get; }
    }
}
