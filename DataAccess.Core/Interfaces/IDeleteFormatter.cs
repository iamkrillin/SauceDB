using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
#pragma warning disable 1591

namespace DataAccess.Core.Interfaces
{
    public interface IDeleteFormatter
    {
        string FormatDelete(Expression expression, out Dictionary<string, Object> parameters);
    }
}
