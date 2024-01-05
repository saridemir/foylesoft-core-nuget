using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Models
{
    public  class AzureConfigInfo
    {
        public string ClientIdB2C { get; set; }
        public string ClientSecret { get; set; }        
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string SignInPolicyId { get; set; }
        public string InstrumentationKey { get; set; }
        public string AzureRedisConnection { get; set; }
        public string StorageAccountConnection { get; set; }
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string StorageContainerName { get; set; }
        
        public string ConnectionStringMySQL { get; set; }
        public string SmsConnection { get; set; }
        public string Domain { get; set; }
        public string BaseApi { get => "/api/"; }

        public List<CosmosDbInfo> CosmosDb { get; set; }
    }
    public class CosmosDbInfo 
    {
        public string Country { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
