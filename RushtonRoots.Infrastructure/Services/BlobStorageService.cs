using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using RushtonRoots.Domain.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace RushtonRoots.Infrastructure.Services;

public class BlobStorageService : IBlobStorageService
{
    // Default thumbnail configuration constants
    private static readonly List<ThumbnailSize> DefaultThumbnailSizes = new()
    {
        new ThumbnailSize { Name = "small", Width = 200, Height = 200 },
        new ThumbnailSize { Name = "medium", Width = 400, Height = 400 }
    };
    private const int DefaultThumbnailQuality = 85;

    private readonly BlobContainerClient _containerClient;
    private readonly string _containerName;
    private readonly IConfiguration _configuration;
    private readonly List<ThumbnailSize> _thumbnailSizes;
    private readonly int _thumbnailQuality;

    public BlobStorageService(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "rushtonroots-files";
        
        // Load thumbnail configuration with defaults
        _thumbnailSizes = configuration.GetSection("AzureBlobStorage:ThumbnailSizes").Get<List<ThumbnailSize>>() 
            ?? DefaultThumbnailSizes;
        
        _thumbnailQuality = configuration.GetValue<int>("AzureBlobStorage:ThumbnailQuality", DefaultThumbnailQuality);
        
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
        
        // Also delete all associated thumbnails
        foreach (var size in _thumbnailSizes)
        {
            var thumbnailBlobName = $"thumbnails/{size.Name}/{blobName}";
            var thumbnailClient = _containerClient.GetBlobClient(thumbnailBlobName);
            await thumbnailClient.DeleteIfExistsAsync();
        }
    }

    public async Task<Dictionary<string, string>> GenerateThumbnailsAsync(string originalBlobName, Stream originalStream)
    {
        var thumbnailUrls = new Dictionary<string, string>();
        
        // Reset stream position
        originalStream.Position = 0;
        
        try
        {
            // Load the image using ImageSharp
            using var image = await Image.LoadAsync(originalStream);
            
            // Generate thumbnails for each configured size
            foreach (var size in _thumbnailSizes)
            {
                var thumbnailBlobName = $"thumbnails/{size.Name}/{originalBlobName}";
                var thumbnailUrl = await GenerateSingleThumbnailAsync(image, thumbnailBlobName, size.Width, size.Height);
                thumbnailUrls[size.Name] = thumbnailUrl;
            }
        }
        catch (Exception ex)
        {
            // If image processing fails, provide detailed error information
            throw new InvalidOperationException(
                $"Failed to generate thumbnails for '{originalBlobName}'. " +
                $"Error: {ex.GetType().Name} - {ex.Message}", 
                ex);
        }
        
        return thumbnailUrls;
    }
    
    private async Task<string> GenerateSingleThumbnailAsync(Image image, string thumbnailBlobName, int width, int height)
    {
        using var thumbnailStream = new MemoryStream();
        
        // Clone the image and resize
        using (var thumbnail = image.Clone(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Max // Maintains aspect ratio
        })))
        {
            // Save as JPEG with configured quality
            var encoder = new JpegEncoder { Quality = _thumbnailQuality };
            await thumbnail.SaveAsync(thumbnailStream, encoder);
        }
        
        // Upload the thumbnail
        thumbnailStream.Position = 0;
        var blobClient = _containerClient.GetBlobClient(thumbnailBlobName);
        var blobHttpHeaders = new BlobHttpHeaders { ContentType = "image/jpeg" };
        await blobClient.UploadAsync(thumbnailStream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
        
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
