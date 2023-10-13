using FoyleSoft.AzureCore.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace FoyleSoft.AzureCore.Implementations
{
    public abstract class MongoCollectionRepositoryAsync : IMongoCollectionRepositoryAsync
    {
        protected readonly IMongoDatabase _database;
        protected readonly MongoClient _mongoClient;
        private readonly string _connectionCountry;
        public string ConnectionCountry { get => _connectionCountry; }

        public MongoCollectionRepositoryAsync(string country, IAzureConfigurationService azureConfigurationService)
        {
            _connectionCountry= country;
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
        public async Task AddAsync(Guid reference, List<BsonDocument> data)
        {
            var collection = _database.GetCollection<BsonDocument>(reference.ToString());
            await collection.InsertManyAsync(data);
        }
        public async Task AddToCollectionAsync(string collectionName, BsonDocument data)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            await collection.InsertOneAsync(data);
        }
        public async Task<IMongoCollection<BsonDocument>> GetCollectionAsync(Guid reference)
        {
            return _database.GetCollection<BsonDocument>(reference.ToString());
        }
        public async Task<IMongoCollection<BsonDocument>> GetCollectionAsync(string reference)
        {
            return _database.GetCollection<BsonDocument>(reference);
        }

        public async Task<long> CountAsync(Guid reference)
        {
            var collection = _database.GetCollection<BsonDocument>(reference.ToString());
            return await collection.EstimatedDocumentCountAsync();
        }

        public async Task<long> CountAsync(string reference)
        {
            var collection = _database.GetCollection<BsonDocument>(reference);
            return await collection.EstimatedDocumentCountAsync();
        }

        public async Task DeleteAsync(Guid reference)
        {
            await _database.DropCollectionAsync(reference.ToString());
        }

        public async Task<List<BsonDocument>> GetAsync(Guid reference)
        {
            var collection = _database.GetCollection<BsonDocument>(reference.ToString());
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<BsonDocument>> GetAsync(string reference)
        {
            var collection = _database.GetCollection<BsonDocument>(reference);
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<List<BsonDocument>> FindAllAsync(Guid reference, Expression<Func<BsonDocument, bool>> match)
        {
            var collection = _database.GetCollection<BsonDocument>(reference.ToString());
            return await collection.Find(match).ToListAsync();
        }

        public async Task<List<BsonDocument>> FindAllAsync(string reference, Expression<Func<BsonDocument, bool>> match)
        {
            var collection = _database.GetCollection<BsonDocument>(reference);
            return await collection.Find(match).ToListAsync();
        }

        public async Task DeleteDocumentAsync(Guid reference, BsonDocument data)
        {
            var collection = _database.GetCollection<BsonDocument>(reference.ToString());
            await collection.DeleteOneAsync(data);
        }

        public async Task DeleteDocumentAsync(string reference, BsonDocument data)
        {
            var collection = _database.GetCollection<BsonDocument>(reference);
            await collection.DeleteOneAsync(data);
        }

        public async Task<bool> UpdateDocumentAsync(string reference, BsonDocument oldData, BsonDocument newData)
        {
            var collection = _database.GetCollection<BsonDocument>(reference);
            var response = await collection.ReplaceOneAsync(oldData, newData);

            return response.IsAcknowledged && response.ModifiedCount == 1;
        }

        public async Task<long> CountDocumentAsync(Guid reference, Expression<Func<BsonDocument, bool>> match)
        {
            var collection = _database.GetCollection<BsonDocument>(reference.ToString());
            var filter = Builders<BsonDocument>.Filter;
            return await collection.CountDocumentsAsync(filter.Where(match));
        }

        public async Task<long> CountDocumentAsync(string reference, Expression<Func<BsonDocument, bool>> match)
        {
            var collection = _database.GetCollection<BsonDocument>(reference);
            var filter = Builders<BsonDocument>.Filter;
            return await collection.CountDocumentsAsync(filter.Where(match));
        }
    }
}