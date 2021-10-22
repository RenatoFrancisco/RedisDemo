using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisDemo.Infra
{
    public interface ICacheProvider
    {
        Task<T> GetFromCacheAsync<T>(string key) where T : class;
        Task SetCacheAsync<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        Task ClearCacheAsync(string key);
    }
}