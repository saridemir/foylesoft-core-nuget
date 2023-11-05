using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IMongoCollectionRepositoryAsync<T> where T : class
    {
        string ConnectionCountry { get; }
        Task AddAsync(Guid reference, List<T> data);
        Task AddToCollectionAsync(string collectionName, T data);
        Task AddOrUpdateCollectionAsync(string collectionName, T data);
        Task<long> CountAsync(Guid reference);
        Task<long> CountAsync(string reference);
        Task<long> CountDocumentAsync(Guid reference, Expression<Func<T, bool>> match);
        Task<long> CountDocumentAsync(string reference, Expression<Func<T, bool>> match);
        Task DeleteAsync(Guid reference);
        Task DeleteDocumentAsync(Guid reference, T data);
        Task DeleteDocumentAsync(string reference, T data);
        Task<List<T>> GetAsync(Guid reference);
        Task<List<T>> GetAsync(string reference);
        Task<List<T>> FindAllAsync(Guid reference, Expression<Func<T, bool>> match);

        Task<List<T>> FindAllAsync(string reference, FilterDefinition<T> filter);
        Task<List<T>> FindAllAsync(string reference, Expression<Func<T, bool>> match);
        Task TruncateDatabaseAsync();
        Task<IMongoCollection<T>> GetCollectionAsync(Guid reference);

        Task<IMongoCollection<T>> GetCollectionAsync(string reference);

        Task<bool> UpdateDocumentAsync(string reference, T newData);
    }
}