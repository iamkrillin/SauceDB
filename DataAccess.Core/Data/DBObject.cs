using DataAccess.Core.Attributes;
using System.Collections.Generic;

namespace DataAccess.Core.Data
{
    /// <summary>
    /// Information about a database table
    /// </summary>
    [BypassValidation]
    public class DBObject
    {
        /// <summary>
        /// the schema the table is in
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// The Table name
        /// </summary>
        [DataField(FieldName = "Table")]
        public string Name { get; set; }

        /// <summary>
        /// The tables columns
        /// </summary>
        [IgnoredField]
        public List<Column> Columns { get; set; }
    }
}
