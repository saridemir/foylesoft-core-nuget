using System;
using System.Collections.Generic;
using System.Text;

namespace FoyleSoft.Core.Implementations
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string QueueCollectionName { get; set; }
        public string SuccessCollectionName { get; set; }
        public string FailCollectionName { get; set; }
    }
}