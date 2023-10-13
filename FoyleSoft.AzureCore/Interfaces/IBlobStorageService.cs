using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(Stream stream, string blobName, string contentType);
        Task<Stream> DownloadFileAsync(string blobName);
        Task<bool> DeleteFileAsync(string blobName);
        Task<string> UploadFileAsync(Stream stream, string containerName, string blobName, string contentType);
        Task<Stream> DownloadFileAsync(string containerName, string blobName);
        Task<bool> DeleteFileAsync(string containerName, string blobName);
    }
}
