using System.Text.Json;

namespace Emma.Api.Helpers.BlobStorageHelper
{
    /// <summary>
    /// Stores images on imgbb (https://api.imgbb.com). The API key is read from configuration ("Imgbb:ApiKey").
    /// imgbb has no delete API for anonymous keys, so deleting only removes the reference on our side.
    /// </summary>
    public class BlobStorageHelper : IBlobStorageHelper
    {
        private const long MaxFileSizeBytes = 8 * 1024 * 1024;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<BlobStorageHelper> _logger;

        public BlobStorageHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<BlobStorageHelper> logger)
        {
            _httpClient = httpClientFactory.CreateClient("imgbb");
            _apiKey = configuration["Imgbb:ApiKey"] ?? "";
            _logger = logger;
        }

        public async Task<string> UploadProfilePictureAsync(Stream fileStream, string fileName)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("Image storage is not configured (Imgbb:ApiKey).");

            using var memory = new MemoryStream();
            await fileStream.CopyToAsync(memory);
            if (memory.Length == 0)
                throw new InvalidOperationException("The uploaded image is empty.");
            if (memory.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("The uploaded image is larger than 8 MB.");

            using var form = new MultipartFormDataContent
            {
                { new StringContent(Convert.ToBase64String(memory.ToArray())), "image" },
                { new StringContent(Path.GetFileNameWithoutExtension(fileName)), "name" }
            };

            var response = await _httpClient.PostAsync($"https://api.imgbb.com/1/upload?key={_apiKey}", form);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("imgbb upload failed ({Status}): {Body}", (int)response.StatusCode, body);
                throw new InvalidOperationException("Uploading the image failed.");
            }

            using var json = JsonDocument.Parse(body);
            var data = json.RootElement.GetProperty("data");
            var url = data.TryGetProperty("display_url", out var display) && !string.IsNullOrEmpty(display.GetString())
                ? display.GetString()
                : data.GetProperty("url").GetString();

            return url ?? throw new InvalidOperationException("imgbb did not return an image URL.");
        }

        public Task<bool> DeleteBlobAsync(string blobName)
        {
            _logger.LogInformation("Removing image reference {BlobName} (imgbb keeps the file).", blobName);
            return Task.FromResult(true);
        }
    }
}
