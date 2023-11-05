using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations.Data
{
    public interface IMongoBaseTable
    {
        ObjectId _id { get; set; }
    }
}
