using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq;
using System.Data.SQLite;
using DataAccess.Core;
using DataAccess.Core.Linq.Common;

namespace DataAccess.SQLite.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public class SQLiteQueryProvider : DBQueryProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteQueryProvider"/> class.
        /// </summary>
        /// <param name="dStore">The d store.</param>
        public SQLiteQueryProvider(IDataStore dStore)
            : base(new SQLiteLanguage(), dStore.GetQueryMapper(), new QueryPolicy(), dStore)
        {
        }
    }
}
