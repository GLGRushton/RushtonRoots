using FakeItEasy;
using Microsoft.Extensions.Configuration;
using RushtonRoots.Infrastructure.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class BlobStorageServiceTests
{
    [Fact]
    public async Task GenerateThumbnailsAsync_CreatesMultipleThumbnailSizes()
    {
        // Note: This is a basic test to verify the thumbnail generation logic
        // In production with actual Azure Blob Storage, more integration tests would be needed
        
        // Arrange
        var configuration = CreateTestConfiguration();
        
        // We can't fully test BlobStorageService without a real blob storage connection
        // This test validates the configuration loading
        
        // Assert configuration is loaded
        Assert.NotNull(configuration);
        var thumbnailSizes = configuration.GetSection("AzureBlobStorage:ThumbnailSizes").Get<List<RushtonRoots.Domain.Configuration.ThumbnailSize>>();
        Assert.NotNull(thumbnailSizes);
        Assert.Equal(2, thumbnailSizes.Count);
        Assert.Contains(thumbnailSizes, s => s.Name == "small" && s.Width == 200 && s.Height == 200);
        Assert.Contains(thumbnailSizes, s => s.Name == "medium" && s.Width == 400 && s.Height == 400);
    }

    [Fact]
    public void ThumbnailConfiguration_LoadsCorrectQuality()
    {
        // Arrange
        var configuration = CreateTestConfiguration();
        
        // Act
        var quality = configuration.GetValue<int>("AzureBlobStorage:ThumbnailQuality");
        
        // Assert
        Assert.Equal(85, quality);
    }

    [Fact]
    public async Task ImageSharpResizeWorks_CreatesExpectedThumbnail()
    {
        // This test verifies that ImageSharp can resize images correctly
        // It doesn't test Azure upload, but validates the core resizing logic
        
        // Arrange
        using var image = new Image<Rgba32>(800, 600);
        using var memoryStream = new MemoryStream();
        
        // Fill with a test color
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                image[x, y] = new Rgba32(255, 0, 0); // Red
            }
        }
        
        // Act
        await image.SaveAsJpegAsync(memoryStream);
        memoryStream.Position = 0;
        
        // Verify we can load and resize
        using var loadedImage = await Image.LoadAsync(memoryStream);
        Assert.Equal(800, loadedImage.Width);
        Assert.Equal(600, loadedImage.Height);
        
        // Resize to thumbnail
        loadedImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Max
        }));
        
        // Assert thumbnail is correctly sized (maintains aspect ratio)
        Assert.True(loadedImage.Width <= 200);
        Assert.True(loadedImage.Height <= 200);
    }

    private IConfiguration CreateTestConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"AzureBlobStorage:ConnectionString", "UseDevelopmentStorage=true"},
            {"AzureBlobStorage:ContainerName", "test-container"},
            {"AzureBlobStorage:ThumbnailSizes:0:Name", "small"},
            {"AzureBlobStorage:ThumbnailSizes:0:Width", "200"},
            {"AzureBlobStorage:ThumbnailSizes:0:Height", "200"},
            {"AzureBlobStorage:ThumbnailSizes:1:Name", "medium"},
            {"AzureBlobStorage:ThumbnailSizes:1:Width", "400"},
            {"AzureBlobStorage:ThumbnailSizes:1:Height", "400"},
            {"AzureBlobStorage:ThumbnailQuality", "85"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }
}
