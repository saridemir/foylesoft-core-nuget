using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FoyleSoft.AzureCore.Implementations.Data
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class MongoBaseTable : IMongoBaseTable
    {
        [BsonId]
        public ObjectId _id { get; set; }
    }
}
