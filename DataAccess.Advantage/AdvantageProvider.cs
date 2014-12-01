using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Linq;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq.Common;

#pragma warning disable 1591

namespace DataAccess.Advantage
{
    public class AdvantageProvider : DBQueryProvider
    {
        public AdvantageProvider(IDataStore dstore)
            : base(new AdvantageLanguage(), dstore.GetQueryMapper(), new QueryPolicy(), dstore)
        {
        }
    }
}
