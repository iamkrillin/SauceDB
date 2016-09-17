using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;

namespace DataAccess.Core
{
    /// <summary>
    /// Indicates that one or more conventions about a data field need to be changed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataFieldAttribute : Attribute
    {
        /// <summary>
        /// Defaults to property name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Indicates if the field should be set on insert or not, defaults to true
        /// </summary>
        public bool SetOnInsert { get; set; }

        /// <summary>
        /// Indicates if a field should be loaded when querying, defaults to true
        /// </summary>
        public bool LoadField { get; set; }

        /// <summary>
        /// Indicates the Data type of the data store field, this string is passed straight to the underlying datastore unchanged
        /// </summary>
        public string FieldTypeString { get; set; }

        /// <summary>
        /// Allows the user to override a field type mapping in a cross db manner
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// Set to override the field length from whatever the default happens to be, not supported for all data types
        /// -1 means 'use the default'
        /// </summary>
        public int FieldLength { get; set; }

        /// <summary>
        /// Indicates the type that this field relates to (FK relationship), defaults to null
        /// </summary>
        public Type PrimaryKeyType { get; set; }

        /// <summary>
        /// Indicates the type of fk relationship to generate
        /// </summary>
        public ForeignKeyType RelationshipType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataFieldAttribute()
        {
            FieldLength = -1; //wanted to use nullable, no go with attributes
            SetOnInsert = true;
            LoadField = true;
            RelationshipType = ForeignKeyType.Cascade;
            FieldType = FieldType.Default;
        }
    }
}
