// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class ScopedDictionary<TKey, TValue>
    {
        ScopedDictionary<TKey, TValue> previous;
        Dictionary<TKey, TValue> map;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="previous">The previous.</param>
        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous)
        {
            this.previous = previous;
            this.map = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="previous">The previous.</param>
        /// <param name="pairs">The pairs.</param>
        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
            : this(previous)
        {
            foreach (var p in pairs)
            {
                this.map.Add(p.Key, p.Value);
            }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(TKey key, TValue value)
        {
            this.map.Add(key, value);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope.previous)
            {
                if (scope.map.TryGetValue(key, out value))
                    return true;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            for (ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope.previous)
            {
                if (scope.map.ContainsKey(key))
                    return true;
            }
            return false;
        }
    }
}