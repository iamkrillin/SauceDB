// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryCommand
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        public string CommandText { get; private set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public ReadOnlyCollection<QueryParameter> Parameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand"/> class.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="parameters">The parameters.</param>
        public QueryCommand(string commandText, IEnumerable<QueryParameter> parameters)
        {
            this.CommandText = commandText;
            this.Parameters = parameters.ToReadOnly();
        }
    }
}
