using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;

namespace DataAccess.Xamarin.iOS.Conversion
{
    /// <summary>
    /// A generic type converter
    /// </summary>
    public class iOSCLRConverter : IConvertToCLR
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
                if (type == typeof(Boolean))
                {
                    string test = p.ToString();
                    toReturn = test.Equals("TRUE", StringComparison.InvariantCultureIgnoreCase) ||
                               test.Equals("T", StringComparison.InvariantCultureIgnoreCase) ||
                               test.Equals("1", StringComparison.InvariantCultureIgnoreCase);
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    toReturn = type.GetConstructor(new Type[] { type.GetGenericArguments()[0] }).Invoke(new object[] { ConvertToType(p, type.GetGenericArguments()[0]) });
                }
                else if (type == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(p.ToString());
                }
                else if (p.GetType() == typeof(Guid))
                {
                    toReturn = p.ToString();
                }
                else if (type.IsEnum)
                {
                    toReturn = Convert.ChangeType(p, Enum.GetUnderlyingType(type));
                }
                else
                {
                    toReturn = Convert.ChangeType(p, type);
                }
            }

            return toReturn;
        }

        public T ConvertToType<T>(object p)
        {
            throw new NotImplementedException("This method signature is unreliable on iOS");
        }
    }
}
