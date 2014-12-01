using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;

namespace DataAccess.Xamarin.iOS.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class iOSQueryProvider : DBQueryProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="iOSQueryProvider"/> class.
        /// </summary>
        /// <param name="dStore">The d store.</param>
        public iOSQueryProvider(IDataStore dStore)
            : base(new iOSLanguage(), dStore.GetQueryMapper(), new QueryPolicy(), dStore)
        {
        }
    }
}
