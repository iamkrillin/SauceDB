using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Attributes;

namespace DataAccess.Core.Data
{
    /// <summary>
    /// Information about a database column
    /// </summary>
    [BypassValidation]
    public class Column
    {
        /// <summary>
        /// The columns name
        /// </summary>
        [DataField(FieldName = "ColumnName")]
        public string Name { get; set; }

        /// <summary>
        /// The columns data type
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The length of the column
        /// </summary>
        public string ColumnLength { get; set; }

        /// <summary>
        /// The column type with the length specifier if required
        /// </summary>
        public string ResolvedColumnType
        {
            get
            {
                if (!string.IsNullOrEmpty(ColumnLength))
                    return string.Format("{0}({1})", DataType, ColumnLength);
                else
                    return DataType;
            }
        }


        /// <summary>
        /// The default value of the column, as reported by the data store
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// True if the data store reports the column as the primary key
        /// </summary>
        [DataField(FieldName = "PrimaryKey")]
        public bool IsPrimaryKey { get; set; }
    }
}
