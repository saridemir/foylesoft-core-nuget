using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.Core.Implementations.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace FoyleSoft.AzureCore.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        private readonly ConcurrentDictionary<string, SemaphoreSlim> KeyLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public Task<IDistributedCache> CacheAsync => Task.FromResult(_distributedCache);

        public async Task<Dictionary<int, T>> GetCachedQueryDictionaryAsync<T>(string key, IQueryable<T> data) where T : IBaseTable
        {
            key = await GetCacheKey(key);

            var cachedData = await _distributedCache.GetStringAsync(key);
            if (cachedData == null)
            {
                var cacheLock = KeyLocks.GetOrAdd(key, k => new SemaphoreSlim(1));

                await cacheLock.WaitAsync();

                try
                {
                    cachedData = await _distributedCache.GetStringAsync(key);
                    if (cachedData == null)
                    {
                        var result = data.ToDictionary(f => f.Id, f => f);

                        var serializedResult = JsonConvert.SerializeObject(result);
                        await _distributedCache.SetStringAsync(key, serializedResult, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) });

                        return result;
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<Dictionary<int, T>>(cachedData);
                    }
                }
                finally
                {
                    cacheLock.Release();
                    KeyLocks.TryRemove(key, out _);
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<Dictionary<int, T>>(cachedData);
            }
        }

        public async Task<List<T>> GetCachedQueryListAsync<T>(string key, IQueryable<T> data)
        {
            key = await GetCacheKey(key);

            var cachedData = await _distributedCache.GetStringAsync(key);
            if (cachedData == null)
            {
                var cacheLock = KeyLocks.GetOrAdd(key, k => new SemaphoreSlim(1));

                await cacheLock.WaitAsync();

                try
                {
                    cachedData = await _distributedCache.GetStringAsync(key);
                    if (cachedData == null)
                    {
                        var result = data.ToList();

                        var serializedResult = System.Text.Json.JsonSerializer.Serialize(result);
                        await _distributedCache.SetStringAsync(key, serializedResult, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) });

                        return result;
                    }
                    else
                    {
                        return System.Text.Json.JsonSerializer.Deserialize<List<T>>(cachedData);
                    }
                }
                finally
                {
                    cacheLock.Release();
                    KeyLocks.TryRemove(key, out _);
                }
            }
            else
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<T>>(cachedData);
            }
        }

        public async Task<string> SetCachedStringAsync(string key, string data)
        {
            key = await GetCacheKey(key);

            data = data ?? string.Empty;

            var cacheLock = KeyLocks.GetOrAdd(key, k => new SemaphoreSlim(1));

            await cacheLock.WaitAsync();

            try
            {
                await _distributedCache.SetStringAsync(key, data, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) });
                return data;
            }
            finally
            {
                cacheLock.Release();
                KeyLocks.TryRemove(key, out _);
            }
        }
        public async Task<string> GetCachedStringAsync(string key)
        {
            key = await GetCacheKey(key);

            var cachedData = await _distributedCache.GetStringAsync(key);
            return cachedData ?? string.Empty;
        }

        public async Task RemoveCachedStringAsync(string key)
        {
            key = await GetCacheKey(key);

            var cacheLock = KeyLocks.GetOrAdd(key, k => new SemaphoreSlim(1));

            await cacheLock.WaitAsync();

            try
            {
                await _distributedCache.RemoveAsync(key);
            }
            finally
            {
                cacheLock.Release();
                KeyLocks.TryRemove(key, out _);
            }
        }

        private async Task<string> GetCacheKey(string key)
        {
            var cacheKey = "cachePrefix";


            var cacheLock = KeyLocks.GetOrAdd(cacheKey, k => new SemaphoreSlim(1));

            await cacheLock.WaitAsync();

            try
            {
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);
                var cachePrefix = cachedData ?? string.Empty;

                if (string.IsNullOrEmpty(cachePrefix))
                {
                    await _distributedCache.SetStringAsync(cacheKey, Guid.NewGuid().ToString(), new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(12) });

                    cachedData = await _distributedCache.GetStringAsync(cacheKey);

                    cachePrefix = cachedData;
                }

                key = string.Format("{0}_{1}", cachePrefix, key);

                return key;
            }
            finally
            {
                cacheLock.Release();
                KeyLocks.TryRemove(cacheKey, out _);
            }

        }

        public async Task ResetCacheAsync()
        {
            var key = "cachePrefix";
            var value = Guid.NewGuid().ToString();

            var cacheLock = KeyLocks.GetOrAdd(key, k => new SemaphoreSlim(1));

            await cacheLock.WaitAsync();

            try
            {
                await _distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddHours(12) });
            }
            finally
            {
                cacheLock.Release();
                KeyLocks.TryRemove(key, out _);
            }
        }
    }
}
