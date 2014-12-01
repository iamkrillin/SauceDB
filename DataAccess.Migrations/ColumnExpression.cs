using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Migrations
{
    public class ColumnExpression : Expression
    {
        public Type Object { get; set; }
        public string Column { get; set; }
    }
}
