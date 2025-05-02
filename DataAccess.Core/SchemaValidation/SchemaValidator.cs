using DataAccess.Core.Data;
using DataAccess.Core.Data.Results;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Core.Schema
{
    /// <summary>
    /// A generic schema validator
    /// </summary>
    public abstract class SchemaValidator
    {
        private IDataStore _dstore;
        public IDatastoreObjectValidator TableValidator { get; set; }
        public IDatastoreObjectValidator ViewValidator { get; set; }

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

        public SchemaValidator(IDataStore dstore, IDatastoreObjectValidator tValidator, IDatastoreObjectValidator vValidator)
        {
            _dstore = dstore;
            TableValidator = tValidator;
            ViewValidator = vValidator;
            CanRemoveColumns = false;
            CanUpdateColumns = true;
            CanAddColumns = true;
        }

        protected virtual async Task<DBObject> LoadObjectInfo(QueryRow table, QueryData columns)
        {
            DBObject t = new DBObject();
            await ObjectBuilder.SetFieldData(_dstore, table, t);
            t.Columns = new List<Column>();

            foreach (QueryRow row in columns) //all of the columns for all of the tables were returned, so we need to only get the one I'm working on...
            {
                if (row.FieldHasMapping("TableName")) //make sure the table name is present
                {
                    if (row.GetDataForRowField("TableName").ToString().Equals(t.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        row.GetDataForRowField("Schema").ToString().Equals(t.Schema, StringComparison.InvariantCultureIgnoreCase)) //make sure its the right table
                    {
                        if (row.FieldHasMapping("ColumnName"))
                            await AddColumnToDBObject(row, t);
                    }
                }
            }
            return t;
        }

        protected async Task AddColumnToDBObject(QueryRow row, DBObject t)
        {
            Column toAdd = new Column();
            await ObjectBuilder.SetFieldData(_dstore, row, toAdd);
            t.Columns.Add(toAdd);
        }

        protected virtual string GetDataTypeString(QueryRow column)
        {
            string toReturn = "";
            string len = column.GetDataForRowField("ColumnLength").ToString();

            if (!string.IsNullOrEmpty(len))
                toReturn = $"{column.GetDataForRowField("DataType")}({column.GetDataForRowField("ColumnLength")})";
            else
                toReturn = column.GetDataForRowField("DataType").ToString();

            return toReturn;
        }

        public virtual async Task ValidateType(DatabaseTypeInfo typeInfo)
        {
            if (!typeInfo.BypassValidation)
            {
                if (typeInfo.IsView)
                    await ViewValidator.ValidateObject(_dstore.TypeParser, typeInfo);
                else
                    await TableValidator.ValidateObject(_dstore.TypeParser, typeInfo);
            }
        }
    }
}
