using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace Emma.Api.Helpers.BlobStorageHelper
{
    public class BlobStorageHelper : IBlobStorageHelper
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly ILogger<BlobStorageHelper> _logger;

        public BlobStorageHelper(IConfiguration configuration, ILogger<BlobStorageHelper> logger)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "";
            _logger = logger;
        }

        public async Task<string> UploadProfilePictureAsync(Stream fileStream, string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_containerName);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainer.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task<bool> DeleteBlobAsync(string blobName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainer.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                _logger.LogInformation("Blob deleted: {BlobName}", blobName);
                return true;
            }
            _logger.LogWarning("Blob not found for deletion: {BlobName}", blobName);
            return false;
        }
    }
}
