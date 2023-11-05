using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoyleSoft.AzureCore.Extensions;
using Newtonsoft.Json;

namespace FoyleSoft.AzureCore.Implementations
{
    public class AzureConfigurationService : IAzureConfigurationService
    {
        public AzureConfigurationService(IConfiguration configuration)
        {
            AzureConfig = JsonConvert.DeserializeObject<AzureConfigInfo>(configuration.GetSection("AzureConfigNew").Value);
            ClientConfig = JsonConvert.DeserializeObject<ClientConfigInfo>(configuration.GetSection("ClientConfig").Value);
        }
        public AzureConfigInfo AzureConfig { get; set; }

        public ClientConfigInfo ClientConfig { get; set; }
    }
}
