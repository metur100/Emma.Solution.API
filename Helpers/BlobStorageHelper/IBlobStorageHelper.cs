namespace Emma.Api.Helpers.BlobStorageHelper
{
    public interface IBlobStorageHelper
    {
        Task<string> UploadProfilePictureAsync(Stream fileStream, string fileName);
        Task<bool> DeleteBlobAsync(string blobName);
    }
}
