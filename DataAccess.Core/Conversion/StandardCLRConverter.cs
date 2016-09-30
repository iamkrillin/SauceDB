using DataAccess.Core.Interfaces;
using System;
using System.Reflection;

namespace DataAccess.Core
{
    /// <summary>
    /// A generic type converter
    /// </summary>
    public class StandardCLRConverter : IConvertToCLR
    {
        /// <summary>
        /// Converts a data type
        /// </summary>
        /// <param name="p">The object to convert</param>
        /// <param name="type">The type to convert it to</param>
        /// <returns></returns>
        public virtual object ConvertToType(object p, Type type)
        {
            object toReturn = null;
            if (!(p is DBNull))
            {
                if (type == typeof(bool))
                {
                    string test = p.ToString();
                    toReturn = (test.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase) || test.Equals("T", StringComparison.InvariantCultureIgnoreCase) ? true : test.Equals("1", StringComparison.InvariantCultureIgnoreCase));
                }
                else if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    if (type == typeof(DateTimeOffset))
                    {
                        return DateTimeOffset.Parse(p.ToString());
                    }
                    if (p.GetType() == typeof(Guid))
                    {
                        toReturn = p.ToString();
                    }
                    else if (type.IsEnum)
                    {
                        toReturn = Convert.ChangeType(p, Enum.GetUnderlyingType(type));
                    }
                    else if (type != typeof(TimeSpan))
                    {
                        toReturn = Convert.ChangeType(p, type);
                    }
                    else
                    {
                        TimeSpan ts = new TimeSpan();
                        TimeSpan.TryParse(p.ToString(), out ts);
                        toReturn = ts;
                    }
                }
                else
                {
                    Type[] genericArguments = new Type[] { type.GetGenericArguments()[0] };
                    ConstructorInfo constructor = type.GetConstructor(genericArguments);
                    object[] objArray = new object[] { this.ConvertToType(p, type.GetGenericArguments()[0]) };
                    return constructor.Invoke(objArray);
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Converts a data type
        /// </summary>
        /// <typeparam name="T">The type to convert it to</typeparam>
        /// <param name="p">The object to convert</param>
        /// <returns></returns>
        public virtual T ConvertToType<T>(object p)
        {
            object result = ConvertToType(p, typeof(T));

            if (result != null)
                return (T)result;
            else
                return default(T);
        }
    }
}
