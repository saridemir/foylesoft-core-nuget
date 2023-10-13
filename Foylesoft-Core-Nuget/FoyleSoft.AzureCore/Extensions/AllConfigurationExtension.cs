using Azure.Identity;
using FoyleSoft.AzureCore.Models;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Extensions
{
    public static class AllConfigurationExtension
    {
        public static IFunctionsConfigurationBuilder ApplyAllConfiguration(this IFunctionsConfigurationBuilder builder,string jsonFileName, string azureConfigKey) 
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(jsonFileName, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var azureKeyVaultEndpoint = configuration[azureConfigKey];
            if (!string.IsNullOrEmpty(azureKeyVaultEndpoint))
            {
                builder.ConfigurationBuilder.AddAzureKeyVault(new Uri(azureKeyVaultEndpoint), new DefaultAzureCredential());
            }
            return builder;
        }
        
    }
}
