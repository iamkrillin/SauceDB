using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Data
{
    /// <summary>
    /// The type of where to add
    /// </summary>
    public enum WhereType
    {
        /// <summary>
        /// 
        /// </summary>
        LessThan,

        /// <summary>
        /// 
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 
        /// </summary>
        Equal,

        /// <summary>
        /// 
        /// </summary>
        LessThanEqual,

        /// <summary>
        /// 
        /// </summary>
        GreaterThanEqual,

        /// <summary>
        /// %($arg)%
        /// </summary>
        Contains,

        /// <summary>
        /// ($arg)%
        /// </summary>
        StartsWith,

        /// <summary>
        /// %($arg)
        /// </summary>
        EndsWith,

        /// <summary>
        /// 
        /// </summary>
        NotEqual,
    }
}
