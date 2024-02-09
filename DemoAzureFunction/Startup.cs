using Azure.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using FoyleSoft.AzureCore.Extensions;
using FoyleSoft.AzureCore.Interfaces;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using System.Reflection;
using System.Collections.Generic;
using DemoAzureFunction.Mocks;

[assembly: FunctionsStartup(typeof(DemoAzureFunction.Startup))]
namespace DemoAzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {

            builder.ApplyAllConfiguration("local.settings.json", "AzureKeyVaultEndpoint");
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.ApplyAllServices(new List<Assembly> { typeof(MockContext).Assembly },
                new List<Assembly> { typeof(DummyService).Assembly },
                typeof(MockSessionBaseRepositoryAsync<>),
                typeof(MockSessionService),
                Assembly.GetExecutingAssembly(), "AzureConfigAdmin", "AdminConfig", "EmailConfig");

        }
    }
    internal class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo
        {
            Version = "1.0.0",
            Title = "Evide API",
            Description = "Evide API for admin",
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };

        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
    }
}
