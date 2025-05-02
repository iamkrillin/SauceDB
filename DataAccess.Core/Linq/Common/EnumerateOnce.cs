// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumerateOnce<T> : IEnumerable<T>, IEnumerable
    {
        IEnumerable<T> enumerable;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerateOnce&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public EnumerateOnce(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            var en = Interlocked.Exchange(ref enumerable, null);
            if (en != null)
            {
                return en.GetEnumerator();
            }
            throw new Exception("Enumerated more than once.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}