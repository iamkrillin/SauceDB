// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class TopologicalSorter
    {
        /// <summary>
        /// Sorts the specified items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="fnItemsBeforeMe">The fn items before me.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe)
        {
            return Sort<T>(items, fnItemsBeforeMe, null);
        }

        /// <summary>
        /// Sorts the specified items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="fnItemsBeforeMe">The fn items before me.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe, IEqualityComparer<T> comparer)
        {
            HashSet<T> seen = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();
            HashSet<T> done = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();
            List<T> result = new List<T>();
            foreach (var item in items)
            {
                SortItem(item, fnItemsBeforeMe, seen, done, result);
            }
            return result;
        }

        /// <summary>
        /// Sorts the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="fnItemsBeforeMe">The fn items before me.</param>
        /// <param name="seen">The seen.</param>
        /// <param name="done">The done.</param>
        /// <param name="result">The result.</param>
        private static void SortItem<T>(T item, Func<T, IEnumerable<T>> fnItemsBeforeMe, HashSet<T> seen, HashSet<T> done, List<T> result)
        {
            if (!done.Contains(item))
            {
                if (seen.Contains(item))
                {
                    throw new InvalidOperationException("Cycle in topological sort");
                }
                seen.Add(item);
                var itemsBefore = fnItemsBeforeMe(item);
                if (itemsBefore != null)
                {
                    foreach (var itemBefore in itemsBefore)
                    {
                        SortItem(itemBefore, fnItemsBeforeMe, seen, done, result);
                    }
                }
                result.Add(item);
                done.Add(item);
            }
        }
    }
}