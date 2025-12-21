namespace RushtonRoots.Infrastructure.Services;

public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a file to blob storage
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="fileStream">File content stream</param>
    /// <returns>The URL of the uploaded file</returns>
    Task<string> UploadFileAsync(string fileName, string contentType, Stream fileStream);
    
    /// <summary>
    /// Deletes a file from blob storage
    /// </summary>
    /// <param name="blobName">Name of the blob to delete</param>
    Task DeleteFileAsync(string blobName);
    
    /// <summary>
    /// Generates thumbnails for an image at multiple sizes
    /// </summary>
    /// <param name="originalBlobName">Name of the original image blob</param>
    /// <param name="originalStream">Stream of the original image</param>
    /// <returns>Dictionary of thumbnail size names to URLs</returns>
    Task<Dictionary<string, string>> GenerateThumbnailsAsync(string originalBlobName, Stream originalStream);
    
    /// <summary>
    /// Gets a SAS URL for temporary access to a blob
    /// </summary>
    /// <param name="blobName">Name of the blob</param>
    /// <param name="expiresInMinutes">Expiration time in minutes</param>
    /// <returns>SAS URL</returns>
    Task<string> GetSasUrlAsync(string blobName, int expiresInMinutes = 60);
}
