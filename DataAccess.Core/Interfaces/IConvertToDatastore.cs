using DataAccess.Core.Attributes;
using DataAccess.Core.Data;
using System;
using System.Data;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a class that knows how to convert a CLR type to DataStore types
    /// </summary>
    public interface IConvertToDatastore
    {
        void CoerceValue(IDbDataParameter value);
        object CoerceValue(object value);

        /// <summary>
        /// Used when the user wishes to override the datafield type
        /// </summary>
        /// <param name="type">The requested type</param>
        /// <param name="dfi">The rest of the data</param>
        /// <returns></returns>
        string MapFieldType(FieldType type, DataFieldInfo dfi);

        /// <summary>
        /// The Default type mapper for a datastore
        /// </summary>
        /// <param name="type">The data type to map</param>
        /// <param name="dField">The rest of the daata</param>
        /// <returns></returns>
        string MapType(Type type, DataFieldInfo dField);
    }
}
