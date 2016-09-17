using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core
{
    /// <summary>
    /// Allows the table name/schema to be specified
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// Indicates the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Indicates the tables schema
        /// </summary>
        public string Schema { get; set; }
    }
}
