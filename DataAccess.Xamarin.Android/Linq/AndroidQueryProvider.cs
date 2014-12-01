using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;

namespace DataAccess.Xamarin.Android.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class AndroidQueryProvider : DBQueryProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndroidQueryProvider"/> class.
        /// </summary>
        /// <param name="dStore">The d store.</param>
        public AndroidQueryProvider(IDataStore dStore)
            : base(new AndroidLanguage(), dStore.GetQueryMapper(), new QueryPolicy(), dStore)
        {
        }
    }
}
