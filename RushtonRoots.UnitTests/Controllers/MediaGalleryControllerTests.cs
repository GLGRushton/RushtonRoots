using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;

namespace RushtonRoots.UnitTests.Controllers;

public class MediaGalleryControllerTests
{
    private readonly ILogger<MediaGalleryController> _mockLogger;
    private readonly MediaGalleryController _controller;

    public MediaGalleryControllerTests()
    {
        _mockLogger = A.Fake<ILogger<MediaGalleryController>>();
        _controller = new MediaGalleryController(_mockLogger);
    }

    #region Index Action Tests

    [Fact]
    public void Index_WithNoParameters_ReturnsViewWithDefaults()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Media Gallery", viewResult.ViewData["Title"]);
        Assert.Null(viewResult.ViewData["MediaType"]);
        Assert.Equal(1, viewResult.ViewData["Page"]);
        Assert.Equal(24, viewResult.ViewData["PageSize"]);
    }

    [Fact]
    public void Index_WithVideoType_ReturnsViewWithVideoFilter()
    {
        // Act
        var result = _controller.Index(type: "video");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("video", viewResult.ViewData["MediaType"]);
        Assert.Equal(1, viewResult.ViewData["Page"]);
        Assert.Equal(24, viewResult.ViewData["PageSize"]);
    }

    [Fact]
    public void Index_WithPhotoType_ReturnsViewWithPhotoFilter()
    {
        // Act
        var result = _controller.Index(type: "photo");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("photo", viewResult.ViewData["MediaType"]);
    }

    [Fact]
    public void Index_WithCustomPagination_ReturnsViewWithPaginationSettings()
    {
        // Act
        var result = _controller.Index(page: 3, pageSize: 12);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(3, viewResult.ViewData["Page"]);
        Assert.Equal(12, viewResult.ViewData["PageSize"]);
    }

    [Fact]
    public void Index_WithAllParameters_ReturnsViewWithAllSettings()
    {
        // Act
        var result = _controller.Index(type: "video", page: 2, pageSize: 48);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Media Gallery", viewResult.ViewData["Title"]);
        Assert.Equal("video", viewResult.ViewData["MediaType"]);
        Assert.Equal(2, viewResult.ViewData["Page"]);
        Assert.Equal(48, viewResult.ViewData["PageSize"]);
    }

    [Fact]
    public void Index_WithNullType_ReturnsViewWithNullMediaType()
    {
        // Act
        var result = _controller.Index(type: null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewData["MediaType"]);
    }

    [Theory]
    [InlineData(1, 24)]
    [InlineData(5, 12)]
    [InlineData(10, 100)]
    public void Index_WithVariousPaginationParameters_ReturnsViewWithCorrectValues(int page, int pageSize)
    {
        // Act
        var result = _controller.Index(page: page, pageSize: pageSize);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(page, viewResult.ViewData["Page"]);
        Assert.Equal(pageSize, viewResult.ViewData["PageSize"]);
    }

    #endregion

    #region Upload Action Tests

    [Fact]
    public void Upload_ReturnsViewWithTitle()
    {
        // Act
        var result = _controller.Upload();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Upload Media", viewResult.ViewData["Title"]);
    }

    [Fact]
    public void Upload_ReturnsViewResult()
    {
        // Act
        var result = _controller.Upload();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void MediaGalleryController_HasAuthorizeAttribute()
    {
        // Arrange
        var type = typeof(MediaGalleryController);

        // Act
        var attributes = type.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidLogger_InitializesController()
    {
        // Arrange
        var logger = A.Fake<ILogger<MediaGalleryController>>();

        // Act
        var controller = new MediaGalleryController(logger);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion
}
