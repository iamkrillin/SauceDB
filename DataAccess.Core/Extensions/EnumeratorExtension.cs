using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Extensions
{
    public static class EnumeratorExtension
    {
        /// <summary>
        /// this exists in v4 or later, so I'm going to make my own so I can use it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enr"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerator<T> enr)
        {
            List<T> toReturn = new List<T>();
            while (enr.MoveNext())
                toReturn.Add(enr.Current);

            return toReturn;
        }
    }
}
