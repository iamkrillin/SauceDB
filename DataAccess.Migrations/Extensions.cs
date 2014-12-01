using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

public static class Extensions
{
    public static Expression<Func<T, T2>> MakeExpression<T, T2>(this object t, Expression<Func<T, T2>> Expression) where T : new()
    {
        return Expression;
    }
}
