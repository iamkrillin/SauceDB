using DataAccess.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    /// <summary>
    /// Some additional query operators
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// A custom extension to help with paging data
        /// </summary>
        /// <typeparam name="T">The type of data you are querying</typeparam>
        /// <param name="data">The target list to page</param>
        /// <param name="page">The page you want</param>
        /// <param name="numPerPage">The number of items per page you want></param>
        /// <returns></returns>
        public static PageData<T> GetPage<T>(this IQueryable<T> data, int page, int numPerPage)
        {
            PageData<T> toreturn = new PageData<T>();
            int numitems = data.Count();

            int pages = 0;
            if (numitems > 0)
            {
                pages = numitems / numPerPage;

                if (numitems % numPerPage != 0)
                    pages++;
            }

            toreturn.NumPages = pages;
            toreturn.Data = data.Skip((page - 1) * numPerPage).Take(numPerPage);
            return toreturn;
        }
    }
}
