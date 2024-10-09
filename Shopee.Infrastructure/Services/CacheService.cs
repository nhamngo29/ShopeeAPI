using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shopee.Application.Common.Interfaces;
using Shopee.Domain.Configuration;
using StackExchange.Redis;

namespace Shopee.Infrastructure.Services
{
    public class CacheService: ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IMemoryCache _memoryCache;
        private readonly RedisConfiguration _redisConfiguration;
        private readonly List<object> _cacheKeys;
        public CacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer, IMemoryCache memoryCache, RedisConfiguration redisConfiguration)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
            _memoryCache = memoryCache;
            _redisConfiguration = redisConfiguration;
            _cacheKeys = new List<object>();
        }
        public async Task<string?> GetCacheReponseAync(string cacheKey)
        {
            try
            {
                if (IsRedisEnabled())
                {
                    // caching redis
                    var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
                    return cacheResponse;
                }
                else
                {
                    // caching memory
                    if (_memoryCache.TryGetValue(cacheKey, out string result))
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                //_logging.StartLog($"{nameof(GetCacheReponseAync)} {ex.Message}");
                return null;
            }
        }
        private bool IsRedisEnabled()
        {
            return _redisConfiguration.Enabled && _connectionMultiplexer.IsConnected;
        }

        private async IAsyncEnumerable<string> GetKeyAsync(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException("value cannot be null or white space");
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var item in server.Keys(pattern: pattern))
                {
                    yield return item.ToString();
                }
            }
        }
        public async Task SetCacheReponseAync(string cacheKey, object response, TimeSpan timeOut)
        {
            try
            {
                if (response == null)
                    return;

                var serializedResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                if (IsRedisEnabled())
                {
                    await _distributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = timeOut
                    });
                }
                else
                {
                    _memoryCache.Set(cacheKey, serializedResponse, timeOut);
                    _cacheKeys.Add(cacheKey);
                }
            }
            catch (Exception ex)
            {
                //_logging.StartLog($"{nameof(SetCacheReponseAync)} {ex.Message}");
            }
        }
        public void ClearAllCacheKeysInMemory(string pattern)//Xóa key trong memory
        {
            var keys = _cacheKeys.Where(t => t.ToString().StartsWith(pattern));
            foreach (var key in keys)
            {
                _memoryCache.Remove(key);
            }
        }
        public async Task RemoveCacheAsyncReponse(string pattern)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pattern))
                    throw new ArgumentNullException("value cannot be null or white space");
                if (_redisConfiguration.Enabled && _connectionMultiplexer.IsConnected)
                {
                    await foreach (var key in GetKeyAsync(pattern + "*"))
                    {
                        if (_connectionMultiplexer.IsConnected)//kiểm tra connected redis
                        {
                            await _distributedCache.RemoveAsync(key);
                        }
                    }
                }
                else
                {
                    ClearAllCacheKeysInMemory(pattern);
                }
            }
            catch (Exception ex)
            {
                //_logging.StartLog($"{nameof(GetCacheReponseAync)} {ex.Message}");
            }
        }
    }
}
