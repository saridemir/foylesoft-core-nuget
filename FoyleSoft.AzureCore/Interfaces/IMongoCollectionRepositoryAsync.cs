using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IMongoCollectionRepositoryAsync
    {
        string ConnectionCountry { get; }
        Task AddAsync(Guid reference, List<BsonDocument> data);
        Task AddToCollectionAsync(string collectionName, BsonDocument data);
        Task<long> CountAsync(Guid reference);
        Task<long> CountAsync(string reference);
        Task<long> CountDocumentAsync(Guid reference, Expression<Func<BsonDocument, bool>> match);
        Task<long> CountDocumentAsync(string reference, Expression<Func<BsonDocument, bool>> match);
        Task DeleteAsync(Guid reference);
        Task DeleteDocumentAsync(Guid reference, BsonDocument data);
        Task DeleteDocumentAsync(string reference, BsonDocument data);
        Task<List<BsonDocument>> GetAsync(Guid reference);
        Task<List<BsonDocument>> GetAsync(string reference);
        Task<List<BsonDocument>> FindAllAsync(Guid reference, Expression<Func<BsonDocument, bool>> match);
        Task<List<BsonDocument>> FindAllAsync(string reference, Expression<Func<BsonDocument, bool>> match);
        Task TruncateDatabaseAsync();
        Task<IMongoCollection<BsonDocument>> GetCollectionAsync(Guid reference);
        Task<IMongoCollection<BsonDocument>> GetCollectionAsync(string reference);
        Task<bool> UpdateDocumentAsync(string reference, BsonDocument oldData, BsonDocument newData);
    }
}