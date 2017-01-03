using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using System.Collections;
using System.Reflection;
using System.IO;

namespace DataAccess.Core
{
    /// <summary>
    /// Extensions on various things
    /// </summary>
    public static class DataAccessExtensions
    {
        /// <summary>
        /// Determines whether [is system type] [the specified t].
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///   <c>true</c> if [is system type] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSystemType(this Type t)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(t.Namespace))
                result = t.Namespace.StartsWith("System");

            return result;
        }

        /// <summary>
        /// Determines if the type is a dynamic type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsDynamic(this Type t)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(t.Namespace))
                result = t.Namespace.Equals("System.Dynamic");

            return result;
        }

        /// <summary>
        /// Takes a list and creates a new smaller list of the specified size (Copies the items from the old list into the new one)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="numItems"></param>
        /// <returns></returns>
        public static IList GetSmallerList(this IList source, int numItems)
        {
            if (numItems > source.Count) numItems = source.Count;
            ArrayList toReturn = new ArrayList(numItems);

            for (int i = 0; i < numItems; i++)
            {
                toReturn.Add(source[0]); //get first item in list
                source.RemoveAt(0); //remove first item from list
            }

            return toReturn;
        }

        /// <summary>
        /// Returns a string resource
        /// </summary>
        /// <param name="asmb">The assembly to load it from</param>
        /// <param name="resourceName">The resource name</param>
        /// <returns></returns>
        public static string LoadResource(this Assembly asmb, string resourceName)
        {
            string toReturn = "";
            using (Stream s = asmb.GetManifestResourceStream(resourceName))
            using (StreamReader sr = new StreamReader(s))
            {
                toReturn = sr.ReadToEnd();
            }

            return toReturn;
        }
    }
}
