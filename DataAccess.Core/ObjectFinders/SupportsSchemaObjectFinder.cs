using DataAccess.Core.Data;
using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Core.ObjectFinders
{
    /// <summary>
    /// Will find data objects in a datastore that supports schemas (sql server style)
    /// </summary>
    public class SupportsSchemaObjectFinder : IFindDataObjects
    {
        /// <summary>
        /// Returns a dbobject for an object
        /// </summary>
        /// <param name="objects">the objects to search</param>
        /// <param name="typeInfo">The type to lookup for</param>
        /// <returns></returns>
        public DBObject GetObject(IEnumerable<DBObject> objects, DatabaseTypeInfo typeInfo)
        {
            if (!string.IsNullOrEmpty(typeInfo.Schema))
                return objects.Where(R => R.Schema.Equals(typeInfo.UnEscapedSchema, StringComparison.InvariantCultureIgnoreCase) && R.Name.Equals(typeInfo.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            else
                return objects.Where(R => R.Name.Equals(typeInfo.UnescapedTableName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
