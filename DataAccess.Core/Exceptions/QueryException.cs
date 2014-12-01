using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.Core.Exceptions;

namespace DataAccess.Core
{
    /// <summary>
    /// This exception represents an error while executing a query
    /// </summary>
    public class QueryException : Exception
    {
        /// <summary>
        /// The query text that caused the exception
        /// </summary>
        public string QueryText { get; set; }

        /// <summary>
        /// The parameters that were on the command
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryException"/> class.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="command">The command.</param>
        public QueryException(Exception ex, IDbCommand command)
            : base(ex.Message, ex)
        {
            QueryText = command.CommandText;
            Parameters = new List<ParameterInfo>();
            foreach (IDbDataParameter v in command.Parameters)
            {
                Parameters.Add(new ParameterInfo(v));
            }
        }
    }
}
