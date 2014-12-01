using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#pragma warning disable 1591

namespace DataAccess.Core.Linq.Enums
{
    /// <summary>
    /// A kind of SQL join
    /// </summary>
    public enum JoinType
    {
        CrossJoin,
        InnerJoin,
        CrossApply,
        OuterApply,
        LeftOuter,
        SingletonLeftOuter
    }
}
