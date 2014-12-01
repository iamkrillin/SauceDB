using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using Membase;
using Enyim.Caching.Memcached;

namespace DataAccess.Memcached
{
    /// <summary>
    /// Will use memcahed for caching
    /// </summary>
    public class MemcachedProvider<KeyType, StoreType> : ICacheProvider<KeyType, StoreType>
    {
        private TimeSpan _cacheTime;
        private string _bucket;
        private MembaseClient _client;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="cacheTime">The time to store stuff for</param>
        /// <param name="bucket">The bucket.</param>
        /// <param name="password">The password.</param>
        public MemcachedProvider(TimeSpan cacheTime, string bucket, string password)
        {
            _cacheTime = cacheTime;
            _bucket = bucket;
            _client = new MembaseClient(bucket, password);
        }

        /// <summary>
        /// Gets an object.
        /// </summary>
        public StoreType GetObject(KeyType key)
        {
            return _client.Get<StoreType>(key.ToString());
        }

        /// <summary>
        /// Stores an object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="toStore">the data to store.</param>
        public void StoreObject(KeyType key, StoreType toStore)
        {
            _client.Store(StoreMode.Set, key.ToString(), toStore);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            _client.FlushAll();
        }

        public bool ContainsKey(KeyType key)
        {
            return GetObject(key) != null;
        }
    }
}
