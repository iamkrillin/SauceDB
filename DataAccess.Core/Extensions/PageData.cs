using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core.Extensions
{
    /// <summary>
    /// Information for paging data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T>
    {

        /// <summary>
        ///The number of pages
        /// </summary>
        public int NumPages { get; set; }


        /// <summary>
        /// The current data set
        /// </summary>
        public IQueryable<T> Data { get; set; }
    }
}
