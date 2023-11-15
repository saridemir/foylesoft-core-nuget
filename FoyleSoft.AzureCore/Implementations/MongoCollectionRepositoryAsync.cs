using FoyleSoft.AzureCore.Implementations.Data;
using FoyleSoft.AzureCore.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace FoyleSoft.AzureCore.Implementations
{
    public abstract class MongoCollectionRepositoryAsync<T> : IMongoCollectionRepositoryAsync<T> where T : MongoBaseTable
    {
        protected readonly IMongoDatabase _database;
        protected readonly MongoClient _mongoClient;
        private readonly string _connectionCountry;
        public string ConnectionCountry { get => _connectionCountry; }
        public MongoCollectionRepositoryAsync(IAzureConfigurationService azureConfigurationService)
        {
            var config = azureConfigurationService.AzureConfig.CosmosDb.FirstOrDefault();
            if (config == null)
            {
                throw new Exception($"CosmosDb not found");
            }
            _connectionCountry = config.Country;
            _mongoClient = new MongoClient(config.ConnectionString);
            _database = _mongoClient.GetDatabase(config.DatabaseName);

        }
        public MongoCollectionRepositoryAsync(string country, IAzureConfigurationService azureConfigurationService)
        {
            _connectionCountry = country;
            var config = azureConfigurationService.AzureConfig.CosmosDb.FirstOrDefault(f => f.Country == country);
            if (config == null)
            {
                throw new Exception($"MongoCollectionRepositoryAsync County :{country} not found");
            }
            _mongoClient = new MongoClient(config.ConnectionString);
            _database = _mongoClient.GetDatabase(config.DatabaseName);

        }
        public async Task TruncateDatabaseAsync()
        {
            var collectionNames = await _database.ListCollectionNames().ToListAsync();
            foreach (var collectionName in collectionNames)
            {
                await _database.DropCollectionAsync(collectionName);
            }
        }
        public async Task AddAsync(Guid reference, List<T> data)
        {
            var collection = _database.GetCollection<T>(reference.ToString());
            await collection.InsertManyAsync(data);
        }
        public async Task AddToCollectionAsync(string collectionName, T data)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(data);
        }

        public async Task AddOrUpdateCollectionAsync(string collectionName, T data)
        {
            if (data._id == ObjectId.Empty)
                await AddToCollectionAsync(collectionName, data);
            else
                await UpdateDocumentAsync(collectionName, data);
        }

        public async Task<IMongoCollection<T>> GetCollectionAsync(Guid reference)
        {
            return _database.GetCollection<T>(reference.ToString());
        }
        public async Task<IMongoCollection<T>> GetCollectionAsync(string reference)
        {
            return _database.GetCollection<T>(reference);
        }

        public async Task<long> CountAsync(Guid reference)
        {
            var collection = _database.GetCollection<T>(reference.ToString());
            return await collection.EstimatedDocumentCountAsync();
        }

        public async Task<long> CountAsync(string reference)
        {
            var collection = _database.GetCollection<T>(reference);
            return await collection.EstimatedDocumentCountAsync();
        }

        public async Task DeleteAsync(Guid reference)
        {
            await _database.DropCollectionAsync(reference.ToString());
        }

        public async Task<List<T>> GetAsync(Guid reference)
        {
            var collection = _database.GetCollection<T>(reference.ToString());
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<T>> GetAsync(string reference)
        {
            var collection = _database.GetCollection<T>(reference);
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<T>> FindAllAsync(Guid reference, Expression<Func<T, bool>> match)
        {
            var collection = _database.GetCollection<T>(reference.ToString());
            return await collection.Find(match).ToListAsync();
        }

        public async Task<List<T>> FindAllAsync(string reference, Expression<Func<T, bool>> match)
        {
            var collection = _database.GetCollection<T>(reference);
            return await collection.Find(match).ToListAsync();
        }

        public async Task<List<T>> FindAllAsync(string reference, FilterDefinition<T> filter)
        {
            var collection = _database.GetCollection<T>(reference);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task DeleteDocumentAsync(Guid reference, T data)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Where(q => q._id == data._id);

            var collection = _database.GetCollection<T>(reference.ToString());
            await collection.DeleteOneAsync(filter);
        }

        public async Task DeleteDocumentAsync(string reference, T data)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Where(q => q._id == data._id);

            var collection = _database.GetCollection<T>(reference);
            await collection.DeleteOneAsync(filter);
        }

        public async Task<bool> UpdateDocumentAsync(string reference, T data)
        {
            var builder = Builders<T>.Filter;
            var filter = builder.Where(q => q._id == data._id);

            var collection = _database.GetCollection<T>(reference);
            var response = await collection.ReplaceOneAsync(filter, data);

            return response.IsAcknowledged && response.ModifiedCount == 1;
        }

        public async Task<long> CountDocumentAsync(Guid reference, Expression<Func<T, bool>> match)
        {
            var collection = _database.GetCollection<T>(reference.ToString());
            var filter = Builders<T>.Filter;
            return await collection.CountDocumentsAsync(filter.Where(match));
        }

        public async Task<long> CountDocumentAsync(string reference, Expression<Func<T, bool>> match)
        {
            var collection = _database.GetCollection<T>(reference);
            var filter = Builders<T>.Filter;
            return await collection.CountDocumentsAsync(filter.Where(match));
        }
    }
}