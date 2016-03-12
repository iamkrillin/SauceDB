using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using DataAccess.Core.Events;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a class responsible for parsing information from a type
    /// </summary>
    public interface ITypeInformationParser
    {
        /// <summary>
        /// This event will fire just after a type has been parsed
        /// </summary>
        event EventHandler<TypeParsedEventArgs> OnTypeParsed;

        /// <summary>
        /// The data cache provider for the type parser
        /// </summary>
        ICacheProvider<Type, DatabaseTypeInfo> Cache { get; set; }

        /// <summary>
        /// Gets the types fields.
        /// </summary>
        /// <param name="dataType">The type to parse</param>
        /// <returns></returns>
        IEnumerable<DataFieldInfo> GetTypeFields(Type dataType);

        /// <summary>
        /// Gets the primary keys for a type
        /// </summary>
        /// <param name="dataType">The type to parse</param>
        /// <returns></returns>
        IEnumerable<DataFieldInfo> GetPrimaryKeys(Type dataType);

        /// <summary>
        /// Gets a lot of information from a type
        /// </summary>
        /// <param name="type">The type to parse</param>
        /// <returns></returns>
        DatabaseTypeInfo GetTypeInfo(Type type);

        /// <summary>
        /// Gets a lot of information from a type
        /// </summary>
        /// <param name="type">The type to parse</param>
        /// <param name="Validate">if False, the the object will not be passed to the validator</param>
        /// <returns></returns>
        DatabaseTypeInfo GetTypeInfo(Type type, bool Validate);
    }
}
