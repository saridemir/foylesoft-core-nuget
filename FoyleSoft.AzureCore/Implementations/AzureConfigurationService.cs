﻿using FoyleSoft.AzureCore.Interfaces;
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
        public AzureConfigurationService(IConfiguration configuration, string configurationKey , string clientConfigurationKey )
        {
            
            AzureConfig = JsonConvert.DeserializeObject<AzureConfigInfo>(configuration.GetSection(configurationKey).Value);
            ClientConfig = JsonConvert.DeserializeObject<ClientConfigInfo>(configuration.GetSection(clientConfigurationKey).Value);
        }
        public AzureConfigInfo AzureConfig { get; set; }

        public ClientConfigInfo ClientConfig { get; set; }
    }
}
