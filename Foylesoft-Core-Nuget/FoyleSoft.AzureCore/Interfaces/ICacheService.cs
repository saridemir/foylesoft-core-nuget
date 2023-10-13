using FoyleSoft.Core.Implementations.Data;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface ICacheService
    {
        Task<List<T>> GetCachedQueryListAsync<T>(string key, IQueryable<T> data);
        Task<Dictionary<int, T>> GetCachedQueryDictionaryAsync<T>(string key, IQueryable<T> data) where T : IBaseTable;
        Task<string> GetCachedStringAsync(string key);
        Task<string> SetCachedStringAsync(string key, string data);
        Task RemoveCachedStringAsync(string key);
        Task<IDistributedCache> CacheAsync { get; }

        Task ResetCacheAsync();
    }
}
