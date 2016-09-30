using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;

#pragma warning disable 1591

namespace DataAccess.Postgre.Linq
{
    public class PostgreQueryProvider : DBQueryProvider
    {
        public PostgreQueryProvider(IDataStore dstore)
            : base(new PostgreLanguage(), dstore.GetQueryMapper(), new QueryPolicy(), dstore)
        {
        }
    }
}
