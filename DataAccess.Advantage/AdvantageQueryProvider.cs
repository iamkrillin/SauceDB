using DataAccess.Core.Interfaces;
using DataAccess.Core.Linq;
using DataAccess.Core.Linq.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Advantage
{
    public class AdvantageQueryProvider : DBQueryProvider
    {
        public AdvantageQueryProvider(IDataStore dstore)
            : base(new AdvantageLanguage(), dstore.GetQueryMapper(), new QueryPolicy(), dstore)
        {
        }
    }
}
