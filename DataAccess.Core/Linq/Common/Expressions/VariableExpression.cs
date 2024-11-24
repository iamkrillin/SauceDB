﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using DataAccess.Core.Linq.Enums;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq.Common.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="VariableExpression"/> class.
    /// </remarks>
    /// <param name="name">The name.</param>
    /// <param name="type">The type.</param>
    /// <param name="queryType">Type of the query.</param>
    public class VariableExpression(string name, Type type) : Expression((ExpressionType)DbExpressionType.Variable, type)
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; } = name;
    }
}
