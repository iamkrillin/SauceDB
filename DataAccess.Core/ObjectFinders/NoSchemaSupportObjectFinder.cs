using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.ObjectFinders
{
    /// <summary>
    /// Will find data object for store that do not support schemas (sql server style)
    /// Assumes {schema}_{table}
    /// </summary>
    public class NoSchemaSupportObjectFinder : IFindDataObjects
    {
        /// <summary>
        /// Returns a dbobject for an object
        /// </summary>
        /// <param name="objects">the objects to search</param>
        /// <param name="typeInfo">The type to lookup for</param>
        /// <returns></returns>
        public DBObject GetObject(IEnumerable<DBObject> objects, TypeInfo typeInfo)
        {
            if (!string.IsNullOrEmpty(typeInfo.Schema))
                return objects.Where(R => R.Name.Equals(string.Concat(typeInfo.UnEscapedSchema, "_", typeInfo.UnescapedTableName), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            else
                return objects.Where(R => R.Name.Equals(typeInfo.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
