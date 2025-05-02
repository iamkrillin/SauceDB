﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Core.Interfaces
{
    /// <summary>
    /// Represents a query data set
    /// </summary>
    public interface IQueryData : IDisposable, IEnumerable
    {
        /// <summary>
        /// The data fields that are present
        /// </summary>
        Dictionary<string, int> QueryFields { get; set; }

        /// <summary>
        /// Returns if the query executed without error
        /// </summary>
        bool QuerySuccessful { get; }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator<IQueryRow> GetQueryEnumerator();
    }
}
