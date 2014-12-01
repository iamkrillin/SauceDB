using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Linq.Common.Language;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="queryType">Type of the query.</param>
        public QueryParameter(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
