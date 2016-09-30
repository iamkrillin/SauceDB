using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data.Results;

namespace DataAccess.Core.Schema
{
    /// <summary>
    /// A generic schema validator
    /// </summary>
    public abstract class SchemaValidator
    {
        private IDataStore _dstore;

        /// <summary>
        /// The component to use when validating tables
        /// </summary>
        public IDatastoreObjectValidator TableValidator { get; set; }

        /// <summary>
        /// The component to use when validating views
        /// </summary>
        public IDatastoreObjectValidator ViewValidator { get; set; }

        /// <summary>
        /// If false, the schema validator will never remove columns (defaults to false)
        /// </summary>
        public bool CanRemoveColumns
        {
            get
            {
                return TableValidator.CanRemoveColumns;
            }
            set
            {
                ViewValidator.CanRemoveColumns = false;
                TableValidator.CanRemoveColumns = value;
            }
        }

        /// <summary>
        /// If false, the schema validator will never add columns (defaults to true)
        /// </summary>
        public bool CanAddColumns
        {
            get
            {
                return TableValidator.CanAddColumns;
            }
            set
            {
                ViewValidator.CanAddColumns = false;
                TableValidator.CanAddColumns = value;
            }
        }

        /// <summary>
        /// If false, the schema validator will never update columns (defaults to true)
        /// </summary>
        public bool CanUpdateColumns
        {
            get
            {
                return TableValidator.CanUpdateColumns;
            }
            set
            {
                ViewValidator.CanUpdateColumns = false;
                TableValidator.CanUpdateColumns = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaValidator" /> class.
        /// </summary>
        /// <param name="dstore">The dstore.</param>
        /// <param name="tValidator">The table validator.</param>
        /// <param name="vValidator">The view validator.</param>
        public SchemaValidator(IDataStore dstore, IDatastoreObjectValidator tValidator, IDatastoreObjectValidator vValidator)
        {
            _dstore = dstore;
            TableValidator = tValidator;
            ViewValidator = vValidator;
            CanRemoveColumns = false;
            CanUpdateColumns = true;
            CanAddColumns = true;
        }

        /// <summary>
        /// Loads the object info.
        /// </summary>
        /// <param name="tables">The tables.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        protected virtual DBObject LoadObjectInfo(QueryRow table, QueryData columns)
        {
            DBObject t = new DBObject();
            ObjectBuilder.SetFieldData(_dstore, table, t);
            t.Columns = new List<Column>();

            foreach (QueryRow row in columns) //all of the columns for all of the tables were returned, so we need to only get the one I'm working on...
            {
                if (row.FieldHasMapping("TableName")) //make sure the table name is present
                {
                    if (row.GetDataForRowField("TableName").ToString().Equals(t.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        row.GetDataForRowField("Schema").ToString().Equals(t.Schema, StringComparison.InvariantCultureIgnoreCase)) //make sure its the right table
                    {
                        if (row.FieldHasMapping("ColumnName"))
                        {
                            AddColumnToDBObject(row, t);
                        }
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// Adds the column to DB object.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="t">The t.</param>
        /// <param name="row">The row.</param>
        protected void AddColumnToDBObject(QueryRow row, DBObject t)
        {
            Column toAdd = new Column();
            ObjectBuilder.SetFieldData(_dstore, row, toAdd);
            t.Columns.Add(toAdd);
        }

        /// <summary>
        /// Returns a Datatype name string for sql, i.e. varchar(20)
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        protected virtual string GetDataTypeString(QueryRow column)
        {
            string toReturn = "";
            string len = column.GetDataForRowField("ColumnLength").ToString();

            if (!string.IsNullOrEmpty(len))
                toReturn = string.Format("{0}({1})", column.GetDataForRowField("DataType"), column.GetDataForRowField("ColumnLength"));
            else
                toReturn = column.GetDataForRowField("DataType").ToString();

            return toReturn;
        }

        /// <summary>
        /// Performs schema validation/modification to match the type
        /// </summary>
        /// <param name="typeInfo"></param>
        public virtual void ValidateType(DatabaseTypeInfo typeInfo)
        {
            if (!typeInfo.BypassValidation)
            {
                if (typeInfo.IsView)
                    ViewValidator.ValidateObject(typeInfo);
                else
                    TableValidator.ValidateObject(typeInfo);
            }
        }
    }
}
