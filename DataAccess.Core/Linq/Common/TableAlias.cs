using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class TableAlias
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAlias"/> class.
        /// </summary>
        public TableAlias()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "A:" + this.GetHashCode();
        }
    }
}
