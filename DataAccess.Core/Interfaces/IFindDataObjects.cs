using DataAccess.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Defines how to find a data object froma datastore based on a type
    /// </summary>
    public interface IFindDataObjects
    {
        /// <summary>
        /// Returns a dbobject for an object
        /// </summary>
        /// <param name="objects">the objects to search</param>
        /// <param name="typeInfo">The type to lookup for</param>
        /// <returns></returns>
        DBObject GetObject(IEnumerable<DBObject> objects, TypeInfo typeInfo);
    }
}
