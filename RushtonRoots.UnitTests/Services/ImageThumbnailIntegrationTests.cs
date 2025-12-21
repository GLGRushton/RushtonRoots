using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

/// <summary>
/// Integration tests for image thumbnail generation with different formats
/// </summary>
public class ImageThumbnailIntegrationTests
{
    [Fact]
    public async Task ThumbnailGeneration_JPEG_CreatesCorrectlySizedImage()
    {
        // Arrange - Create a test JPEG image
        using var originalImage = CreateTestImage(800, 600, Color.Blue);
        using var originalStream = new MemoryStream();
        await originalImage.SaveAsJpegAsync(originalStream);
        
        // Act - Resize to thumbnail
        originalStream.Position = 0;
        using var loadedImage = await Image.LoadAsync(originalStream);
        loadedImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Max
        }));
        
        using var thumbnailStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 85 };
        await loadedImage.SaveAsync(thumbnailStream, encoder);
        
        // Assert
        Assert.True(loadedImage.Width <= 200);
        Assert.True(loadedImage.Height <= 200);
        Assert.Equal(200, loadedImage.Width); // 800x600 -> 200x150 maintaining aspect ratio
        Assert.Equal(150, loadedImage.Height);
        Assert.True(thumbnailStream.Length > 0);
        Assert.True(thumbnailStream.Length < originalStream.Length); // Thumbnail should be smaller
    }

    [Fact]
    public async Task ThumbnailGeneration_PNG_CreatesCorrectlySizedImage()
    {
        // Arrange - Create a test PNG image
        using var originalImage = CreateTestImage(1024, 768, Color.Green);
        using var originalStream = new MemoryStream();
        await originalImage.SaveAsPngAsync(originalStream);
        
        // Act - Resize to thumbnail
        originalStream.Position = 0;
        using var loadedImage = await Image.LoadAsync(originalStream);
        loadedImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(400, 400),
            Mode = ResizeMode.Max
        }));
        
        using var thumbnailStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 85 };
        await loadedImage.SaveAsync(thumbnailStream, encoder);
        
        // Assert
        Assert.True(loadedImage.Width <= 400);
        Assert.True(loadedImage.Height <= 400);
        Assert.Equal(400, loadedImage.Width); // 1024x768 -> 400x300 maintaining aspect ratio
        Assert.Equal(300, loadedImage.Height);
        Assert.True(thumbnailStream.Length > 0);
    }

    [Fact]
    public async Task ThumbnailGeneration_GIF_CreatesCorrectlySizedImage()
    {
        // Arrange - Create a test GIF image (static, not animated)
        using var originalImage = CreateTestImage(640, 480, Color.Red);
        using var originalStream = new MemoryStream();
        await originalImage.SaveAsGifAsync(originalStream);
        
        // Act - Resize to thumbnail
        originalStream.Position = 0;
        using var loadedImage = await Image.LoadAsync(originalStream);
        loadedImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Max
        }));
        
        using var thumbnailStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 85 };
        await loadedImage.SaveAsync(thumbnailStream, encoder);
        
        // Assert
        Assert.True(loadedImage.Width <= 200);
        Assert.True(loadedImage.Height <= 200);
        Assert.Equal(200, loadedImage.Width); // 640x480 -> 200x150 maintaining aspect ratio
        Assert.Equal(150, loadedImage.Height);
        Assert.True(thumbnailStream.Length > 0);
    }

    [Fact]
    public async Task ThumbnailGeneration_PortraitImage_MaintainsAspectRatio()
    {
        // Arrange - Create a portrait (tall) image
        using var originalImage = CreateTestImage(600, 800, Color.Purple);
        using var originalStream = new MemoryStream();
        await originalImage.SaveAsJpegAsync(originalStream);
        
        // Act - Resize to thumbnail
        originalStream.Position = 0;
        using var loadedImage = await Image.LoadAsync(originalStream);
        loadedImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Max
        }));
        
        // Assert
        Assert.Equal(150, loadedImage.Width); // 600x800 -> 150x200 maintaining aspect ratio
        Assert.Equal(200, loadedImage.Height);
    }

    [Fact]
    public async Task ThumbnailGeneration_SmallImage_EnlargesToFit()
    {
        // Arrange - Create a small image (smaller than thumbnail size)
        using var originalImage = CreateTestImage(100, 100, Color.Yellow);
        using var originalStream = new MemoryStream();
        await originalImage.SaveAsJpegAsync(originalStream);
        
        // Act - Resize to thumbnail (ResizeMode.Max enlarges to fit bounds)
        originalStream.Position = 0;
        using var loadedImage = await Image.LoadAsync(originalStream);
        loadedImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Max
        }));
        
        // Assert - Image should be enlarged to 200x200
        Assert.Equal(200, loadedImage.Width);
        Assert.Equal(200, loadedImage.Height);
    }

    private Image<Rgba32> CreateTestImage(int width, int height, Color color)
    {
        var image = new Image<Rgba32>(width, height);
        
        // Fill with the specified color
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                image[x, y] = color;
            }
        }
        
        return image;
    }
}
