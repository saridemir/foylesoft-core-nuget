using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FoyleSoft.AzureCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IAzureConfigurationService _azureConfigurationService;
        private readonly ICacheService _cacheService;
        public BlobStorageService(ICacheService cacheService, IAzureConfigurationService azureConfigurationService, BlobServiceClient blobServiceClient)
        {
            _cacheService = cacheService;
            _azureConfigurationService = azureConfigurationService;
            _blobServiceClient = blobServiceClient;
        }

        private async Task<string> GenerateSasToken(string blobName)
        {
            var cachedSasToken = await _cacheService.GetCachedStringAsync(blobName);
            if (cachedSasToken == null)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _azureConfigurationService.AzureConfig.StorageContainerName,
                    BlobName = blobName,
                    Resource = "b", // Blob
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(2),
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
                var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_azureConfigurationService.AzureConfig.StorageAccountName, _azureConfigurationService.AzureConfig.StorageAccountKey));
                cachedSasToken = sasToken.ToString();
                await _cacheService.SetCachedStringAsync(blobName, cachedSasToken);
            }
            return cachedSasToken;
        }
        public async Task<string> UploadFileAsync(Stream stream,string blobName, string contentType)
        {
            return await UploadFileAsync(stream, _azureConfigurationService.AzureConfig.StorageContainerName, blobName, contentType);
        }
        public async Task<string> UploadFileAsync(Stream stream,string containerName, string blobName, string contentType)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            BlobUploadOptions options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };
            await blobClient.UploadAsync(stream, options);
            return blobClient.Uri.ToString();
        }
        public async Task<Stream> DownloadFileAsync(string blobName)
        {
            return await DownloadFileAsync(_azureConfigurationService.AzureConfig.StorageContainerName, blobName);
        }
        public async Task<bool> DeleteFileAsync(string blobName) {

            return await DeleteFileAsync(_azureConfigurationService.AzureConfig.StorageContainerName, blobName);
        }
        public async Task<bool> DeleteFileAsync(string containerName, string blobName) 
        {
            var sasToken = GenerateSasToken(blobName);
            var blobUri = new Uri($"https://{_azureConfigurationService.AzureConfig.StorageAccountName}.blob.core.windows.net/{containerName}/{blobName}?{sasToken}");
            var blobClient = new BlobClient(blobUri, new BlobClientOptions());
            try
            {
                if (await blobClient.ExistsAsync())
                {
                    var blobDownloadInfo = await blobClient.DeleteAsync();

                    return true;
                }
                return false;
            }
            catch {
                return false;
            }
        }
        public async Task<Stream> DownloadFileAsync(string containerName,string blobName)
        {
            var sasToken = GenerateSasToken(blobName);
            var blobUri = new Uri($"https://{_azureConfigurationService.AzureConfig.StorageAccountName}.blob.core.windows.net/{containerName}/{blobName}?{sasToken}");
            var blobClient = new BlobClient(blobUri, new BlobClientOptions());
            if (await blobClient.ExistsAsync())
            {
                var blobDownloadInfo = await blobClient.DownloadAsync();

                return blobDownloadInfo.Value.Content;
            }
            return null;
        }
    }
}
