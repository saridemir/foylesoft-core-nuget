using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace FoyleSoft.AzureCore.Implementations
{
    public class GraphApiService : IGraphApiService
    {
        private readonly IAzureConfigurationService _azureConfigurationService;
        private readonly ILogger<GraphApiService> _logger;
        public GraphApiService(IAzureConfigurationService azureConfigurationService,ILogger<GraphApiService> logger)
        {
            _logger = logger;
            _azureConfigurationService = azureConfigurationService;
        }
        public async Task<string> CreateUserAsync(AddGrapUserInfo userInfo)
        {
            using (var httpClient = new HttpClient())
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var str = JsonConvert.SerializeObject(userInfo, serializerSettings);
                var token = await GetAccessTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(str, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://graph.microsoft.com/v1.0/users", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }

        public async Task<string> ReadUserAsync(string userGuid)
        {
            using (var httpClient = new HttpClient())
            {
                var token = await GetAccessTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"https://graph.microsoft.com/v1.0/users/{userGuid}");
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }
        public async Task<List<string>> ReadAllUsersAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<string> UpdateUserAsync(string userGuid, UpdateGrapUserInfo userInfo)
        {
            using (var httpClient = new HttpClient())
            {
                var serializerSettings = new JsonSerializerSettings();
                serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var str = JsonConvert.SerializeObject(userInfo, serializerSettings);
                var token = await GetAccessTokenAsync();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(str, Encoding.UTF8, "application/json");
                var response = await httpClient.PatchAsync($"https://graph.microsoft.com/v1.0/users/{userGuid}", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
        }

        public async Task DeleteUserAsync(string userGuid)
        {
            using (var httpClient = new HttpClient())
            {
                var token = await GetAccessTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await httpClient.DeleteAsync($"https://graph.microsoft.com/v1.0/users/{userGuid}");
            }
        }

        async Task<string> GetAccessTokenAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _azureConfigurationService.AzureConfig.ClientIdB2C },
                    { "client_secret", _azureConfigurationService.AzureConfig.ClientSecret },
                    { "scope", "https://graph.microsoft.com/.default" }
                });

                var response = await httpClient.PostAsync($"https://login.microsoftonline.com/{_azureConfigurationService.AzureConfig.TenantId}/oauth2/v2.0/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation(responseContent);
                var accessToken = responseContent.Split("\"access_token\":\"")[1].Split("\"")[0];
                return accessToken;
            }
        }
    }
}
