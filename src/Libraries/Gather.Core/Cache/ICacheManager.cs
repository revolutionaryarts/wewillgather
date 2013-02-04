namespace Gather.Core.Cache
{
    /// <summary>
    /// Cache manager interface
    /// </summary>
    public interface ICacheManager
    {

        /// <summary>
        /// Gets the object associated with the specified key
        /// </summary>
        /// <typeparam name="T">Cached object type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>The object associated with the cache key</returns>
        T Get<T>(string key);

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Object to cache</param>
        /// <param name="cacheTime">Cache duration</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// Check if a cache object with a certain key exists
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        bool IsSet(string key);

        /// <summary>
        /// Remove cache data by key
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);

        /// <summary>
        /// Remove cache records whose key matches a pattern
        /// </summary>
        /// <param name="pattern">Match pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Remove all cache data
        /// </summary>
        void Clear();

    }
}