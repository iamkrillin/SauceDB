// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReadOnlyExtensions
    {
        /// <summary>
        /// Creates a read only list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> collection)
        {
            ReadOnlyCollection<T> roc = collection as ReadOnlyCollection<T>;
            if (roc == null)
            {
                if (collection == null)
                {
                    roc = EmptyReadOnlyCollection<T>.Empty;
                }
                else
                {
                    roc = new List<T>(collection).AsReadOnly();
                }
            }
            return roc;
        }
    }
}
