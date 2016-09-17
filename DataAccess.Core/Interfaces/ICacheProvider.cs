using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Core
{
    /// <summary>
    /// Represents a caching mechanism
    /// </summary>
    /// <typeparam name="KeyType">The type of the object key</typeparam>
    /// <typeparam name="StoreType">What you are storing</typeparam>
    public interface ICacheProvider<KeyType, StoreType>
    {
        /// <summary>
        /// Retrieves an object from the cache, returns default(StoreType) if the item was not found
        /// </summary>
        /// <param name="key">the key to lookup</param>
        /// <returns></returns>
        StoreType GetObject(KeyType key);

        /// <summary>
        /// WIll store the object into the cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="toStore"></param>
        void StoreObject(KeyType key, StoreType toStore);

        /// <summary>
        /// Removes all data from this cache
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Determines if this cache contains the key specified
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool ContainsKey(KeyType key);
    }
}
