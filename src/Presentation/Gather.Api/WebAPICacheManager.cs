using Gather.Core.Cache;

namespace Gather.Api
{
    public class WebAPICacheManager : ICacheManager
    {
        public T Get<T>(string key)
        {
            return default(T);
        }

        public void Set(string key, object data, int cacheTime)
        {
        }

        public bool IsSet(string key)
        {
            // by returning false here, the caller should never request anything from cache
            return false;
        }

        public void Remove(string key)
        {
        }

        public void RemoveByPattern(string pattern)
        {
        }

        public void Clear()
        {
        }
    }
}