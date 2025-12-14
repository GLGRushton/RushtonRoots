using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace RushtonRoots.Infrastructure.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "rushtonroots-files";
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Blob Storage connection string is not configured");
        }
        
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        
        // Create container if it doesn't exist
        _containerClient.CreateIfNotExists(PublicAccessType.None);
    }

    public async Task<string> UploadFileAsync(string fileName, string contentType, Stream fileStream)
    {
        // Generate unique blob name
        var blobName = $"{Guid.NewGuid()}_{fileName}";
        var blobClient = _containerClient.GetBlobClient(blobName);
        
        // Upload the file
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
        
        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> GenerateThumbnailAsync(string originalBlobName, Stream originalStream)
    {
        // For now, we'll use a simple approach - just upload a smaller version
        // In a production environment, you'd want to use an image processing library like ImageSharp
        var thumbnailBlobName = $"thumbnails/{originalBlobName}";
        var blobClient = _containerClient.GetBlobClient(thumbnailBlobName);
        
        // Reset stream position
        originalStream.Position = 0;
        
        // Upload as-is for now (in production, resize the image here)
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" };
        await blobClient.UploadAsync(originalStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
        
        return blobClient.Uri.ToString();
    }

    public async Task<string> GetSasUrlAsync(string blobName, int expiresInMinutes = 60)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException($"Blob {blobName} not found");
        }
        
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes)
        };
        
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
