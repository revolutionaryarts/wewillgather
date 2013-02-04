using System;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace Gather.Core.Cache
{
    public class MemoryCacheManager : ICacheManager
    {

        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// Gets the object associated with the specified key
        /// </summary>
        /// <typeparam name="T">Cached object type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>The object associated with the cache key</returns>
        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Object to cache</param>
        /// <param name="cacheTime">Cache duration</param>
        public void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy { AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime) };
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// Check if a cache object with a certain key exists
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// Remove cache data by key
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Remove cache records whose key matches a pattern
        /// </summary>
        /// <param name="pattern">Match pattern</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = (from item in Cache where regex.IsMatch(item.Key) select item.Key).ToList();

            foreach (string key in keysToRemove)
                Remove(key);
        }

        /// <summary>
        /// Remove all cache data
        /// </summary>
        public void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

    }
}