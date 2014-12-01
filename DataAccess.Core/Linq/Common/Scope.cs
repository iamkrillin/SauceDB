using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Common.Expressions;
using DataAccess.Core.Linq.Common.Translation;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class Scope
    {
        Scope outer;
        ParameterExpression fieldReader;
        internal TableAlias Alias { get; private set; }
        Dictionary<string, int> nameMap;

        internal Scope(Scope outer, ParameterExpression fieldReader, TableAlias alias, IEnumerable<ColumnDeclaration> columns)
        {
            this.outer = outer;
            this.fieldReader = fieldReader;
            this.Alias = alias;
            this.nameMap = columns.Select((c, i) => new { c, i }).ToDictionary(x => x.c.Name, x => x.i);
        }

        internal bool TryGetValue(ColumnExpression column, out ParameterExpression fieldReader, out int ordinal)
        {
            for (Scope s = this; s != null; s = s.outer)
            {
                if (column.Alias == s.Alias && this.nameMap.TryGetValue(column.Name, out ordinal))
                {
                    fieldReader = this.fieldReader;
                    return true;
                }
            }
            fieldReader = null;
            ordinal = 0;
            return false;
        }
    }
}
