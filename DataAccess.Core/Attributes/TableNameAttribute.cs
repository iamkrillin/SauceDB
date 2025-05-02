using System;

namespace DataAccess.Core.Attributes
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
