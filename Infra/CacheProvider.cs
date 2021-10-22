using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisDemo.Infra
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _cache;

        public CacheProvider(IDistributedCache cache) => _cache = cache;

        public async Task<T> GetFromCacheAsync<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse is null ? default : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task SetCacheAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            var response = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, response , options);
        }

        public async Task ClearCacheAsync(string key) => await _cache.RemoveAsync(key);
    }
}