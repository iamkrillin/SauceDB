using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data.Results;

namespace DataAccess.Core
{
    /// <summary>
    /// Some helper functions used throughout the library
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Loads the object info.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="table">The table.</param>
        /// <param name="dstore">The datastore to look in</param>
        /// <returns></returns>
        public static DBObject LoadObjectInfo(IDataStore dstore, IQueryRow table, List<IQueryRow> columns)
        {
            DBObject t = new DBObject();
            ObjectBuilder.SetFieldData(dstore, table, t);
            t.Columns = new List<Column>();

            foreach (IQueryRow row in columns) //all of the columns for all of the tables were returned, so we need to only get the one I'm working on...
            {
                row.ResetUsed();
                if (row.FieldHasMapping("TableName")) //make sure the table name is present
                {
                    string tablename = row.GetDataForRowField("TableName") as string;
                    string schema = row.GetDataForRowField("Schema") as string;

                    if (tablename.Equals(t.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (schema != null && t.Schema != null)
                        {
                            if (schema.Equals(t.Schema, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (row.FieldHasMapping("ColumnName"))
                                {
                                    AddColumn(dstore, row, t);
                                }
                            }
                        }
                        else if (schema == null && t.Schema == null)
                        {
                            if (row.FieldHasMapping("ColumnName"))
                            {
                                AddColumn(dstore, row, t);
                            }
                        }
                    }
                }
            }

            return t;
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="column">The column</param>
        /// <param name="t">The data object</param>
        /// <param name="dstore">The datastore to look in</param>
        private static void AddColumn(IDataStore dstore, IQueryRow column, DBObject t)
        {
            Column toAdd = new Column();
            ObjectBuilder.SetFieldData(dstore, column, toAdd);
            t.Columns.Add(toAdd);
        }
    }
}
