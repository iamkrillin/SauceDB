using System;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a class responsible for converting types from the database into CLR types
    /// </summary>
    public interface IConvertToCLR
    {
        /// <summary>
        /// Converts a data type
        /// </summary>
        /// <param name="p">The object to convert</param>
        /// <param name="type">The type to convert it to</param>
        /// <returns></returns>
        object ConvertToType(object p, Type type);

        /// <summary>
        /// Converts a data type
        /// </summary>
        /// <typeparam name="T">The type to convert it to</typeparam>
        /// <param name="p">The object to convert</param>
        /// <returns></returns>
        T ConvertToType<T>(object p);
    }
}
