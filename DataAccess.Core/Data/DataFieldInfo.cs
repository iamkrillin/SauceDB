using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DataAccess.Core.Attributes;

namespace DataAccess.Core.Data
{
    /// <summary>
    /// Information about a field on a Type
    /// </summary>
    [Serializable]
    public class DataFieldInfo
    {
        /// <summary>
        /// Default constructor, initializes ForeignKeyType=Cascade
        /// </summary>
        public DataFieldInfo()
        {
            ForeignKeyType = ForeignKeyType.Cascade;
            DataFieldType = FieldType.Default;
        }

        /// <summary>
        /// The overridden size of the field
        /// </summary>
        public int? FieldLength { get; set; }

        /// <summary>
        /// The user has provided a string to specify the field type, only honored if DataFieldType='UserString'
        /// </summary>
        public string DataFieldString { get; set; }

        /// <summary>
        /// The user specified field mapping
        /// </summary>
        public FieldType DataFieldType { get; set; }

        /// <summary>
        /// The parsed default value
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// The resolved field name for the column in the data store
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// The resolved field name for the column in the data store, surrounded by escape characters
        /// </summary>
        public string EscapedFieldName { get; set; }

        /// <summary>
        /// The name of the property on the object
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Indicates if the field was resolved as a primary key
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// Indicates the field was marked for inserting
        /// </summary>
        public bool SetOnInsert { get; set; }

        /// <summary>
        /// Indicates the field was marked for loading
        /// </summary>
        public bool LoadField { get; set; }

        /// <summary>
        /// The type that was indicated to contain a primary key for this field
        /// </summary>
        public Type PrimaryKeyType { get; set; }

        /// <summary>
        /// The data type of this property
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// The type of the foreign key relationship (if applicable)
        /// </summary>
        public ForeignKeyType ForeignKeyType { get; set; }

        /// <summary>
        /// Call to get the value of the property
        /// object (instance), null
        /// </summary>
        public Func<object, object[], object> Getter { get; set; }

        /// <summary>
        /// Call to set the value of the property
        /// object (instance), value (what to set it to) , null
        /// </summary>
        public Action<object, object, object[]> Setter { get; set; }

    }
}
