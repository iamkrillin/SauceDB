using System.Collections.Concurrent;

namespace DataAccess.Core
{
    /// <summary>
    /// Caches data in a dictionary
    /// </summary>
    /// <typeparam name="KeyType"></typeparam>
    /// <typeparam name="StoreType"></typeparam>
    public class DictionaryCacheProvider<KeyType, StoreType>
    {
        private ConcurrentDictionary<KeyType, StoreType> _cache;

        /// <summary>
        /// </summary>
        public DictionaryCacheProvider()
        {
            _cache = new ConcurrentDictionary<KeyType, StoreType>();
        }

        /// <summary>
        /// Gets an object from the cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public StoreType GetObject(KeyType key)
        {
            StoreType toReturn = default(StoreType);
            if (_cache.ContainsKey(key))
                _cache.TryGetValue(key, out toReturn);

            return toReturn;
        }

        /// <summary>
        /// Stores an object in the cache
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="toStore">what to store.</param>
        public bool StoreObject(KeyType key, StoreType toStore)
        {
            return _cache.TryAdd(key, toStore);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        public bool ContainsKey(KeyType type)
        {
            return _cache.ContainsKey(type);
        }
    }
}
