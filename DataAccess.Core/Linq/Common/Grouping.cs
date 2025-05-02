// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// A Grouping parser
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        TKey key;
        IEnumerable<TElement> group;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grouping&lt;TKey, TElement&gt;"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="group">The group.</param>
        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            this.key = key;
            this.group = group;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public TKey Key
        {
            get { return this.key; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            if (!(group is List<TElement>))
                group = group.ToList();
            return this.group.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.group.GetEnumerator();
        }
    }
}