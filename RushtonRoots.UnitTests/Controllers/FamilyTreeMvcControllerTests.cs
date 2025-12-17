using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Web.Controllers;
using Xunit;
using FakeItEasy;

namespace RushtonRoots.UnitTests.Controllers;

public class FamilyTreeMvcControllerTests
{
    private readonly ILogger<FamilyTreeMvcController> _mockLogger;
    private readonly FamilyTreeMvcController _controller;

    public FamilyTreeMvcControllerTests()
    {
        _mockLogger = A.Fake<ILogger<FamilyTreeMvcController>>();
        _controller = new FamilyTreeMvcController(_mockLogger);
    }

    #region Index Action Tests

    [Fact]
    public void Index_WithNoParameters_ReturnsViewWithDefaults()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Family Tree", viewResult.ViewData["Title"]);
        Assert.Null(viewResult.ViewData["PersonId"]);
        Assert.Equal("descendant", viewResult.ViewData["ViewMode"]);
        Assert.Equal(3, viewResult.ViewData["Generations"]);
    }

    [Fact]
    public void Index_WithPersonId_ReturnsViewWithPersonId()
    {
        // Act
        var result = _controller.Index(personId: 42);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(42, viewResult.ViewData["PersonId"]);
        Assert.Equal("descendant", viewResult.ViewData["ViewMode"]);
        Assert.Equal(3, viewResult.ViewData["Generations"]);
    }

    [Fact]
    public void Index_WithPedigreeView_ReturnsViewWithPedigreeMode()
    {
        // Act
        var result = _controller.Index(view: "pedigree");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("pedigree", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithFanView_ReturnsViewWithFanMode()
    {
        // Act
        var result = _controller.Index(view: "fan");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("fan", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithDescendantView_ReturnsViewWithDescendantMode()
    {
        // Act
        var result = _controller.Index(view: "descendant");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("descendant", viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_WithCustomGenerations_ReturnsViewWithGenerationsCount()
    {
        // Act
        var result = _controller.Index(generations: 5);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(5, viewResult.ViewData["Generations"]);
    }

    [Fact]
    public void Index_WithAllParameters_ReturnsViewWithAllSettings()
    {
        // Act
        var result = _controller.Index(personId: 123, view: "pedigree", generations: 4);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Family Tree", viewResult.ViewData["Title"]);
        Assert.Equal(123, viewResult.ViewData["PersonId"]);
        Assert.Equal("pedigree", viewResult.ViewData["ViewMode"]);
        Assert.Equal(4, viewResult.ViewData["Generations"]);
    }

    [Fact]
    public void Index_WithNullView_UsesDefaultViewMode()
    {
        // Act
        var result = _controller.Index(view: null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("descendant", viewResult.ViewData["ViewMode"]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void Index_WithVariousGenerations_ReturnsViewWithCorrectGenerations(int generations)
    {
        // Act
        var result = _controller.Index(generations: generations);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(generations, viewResult.ViewData["Generations"]);
    }

    [Theory]
    [InlineData(1, "descendant", 3)]
    [InlineData(5, "pedigree", 4)]
    [InlineData(10, "fan", 2)]
    [InlineData(null, "descendant", 3)]
    public void Index_WithVariousCombinations_ReturnsViewWithCorrectValues(int? personId, string view, int generations)
    {
        // Act
        var result = _controller.Index(personId: personId, view: view, generations: generations);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(personId, viewResult.ViewData["PersonId"]);
        Assert.Equal(view, viewResult.ViewData["ViewMode"]);
        Assert.Equal(generations, viewResult.ViewData["Generations"]);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public void FamilyTreeMvcController_HasAuthorizeAttribute()
    {
        // Arrange
        var type = typeof(FamilyTreeMvcController);

        // Act
        var attributes = type.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }

    #endregion

    #region Route Tests

    [Fact]
    public void Index_HasRouteAttribute()
    {
        // Arrange
        var method = typeof(FamilyTreeMvcController).GetMethod(nameof(FamilyTreeMvcController.Index));

        // Act
        var routeAttribute = method?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.RouteAttribute), true)
            .FirstOrDefault() as Microsoft.AspNetCore.Mvc.RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("FamilyTree", routeAttribute.Template);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidLogger_InitializesController()
    {
        // Arrange
        var logger = A.Fake<ILogger<FamilyTreeMvcController>>();

        // Act
        var controller = new FamilyTreeMvcController(logger);

        // Assert
        Assert.NotNull(controller);
    }

    #endregion

    #region ViewData Tests

    [Fact]
    public void Index_AlwaysSetsTitle()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(viewResult.ViewData.ContainsKey("Title"));
        Assert.NotNull(viewResult.ViewData["Title"]);
    }

    [Fact]
    public void Index_AlwaysSetsViewMode()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(viewResult.ViewData.ContainsKey("ViewMode"));
        Assert.NotNull(viewResult.ViewData["ViewMode"]);
    }

    [Fact]
    public void Index_AlwaysSetsGenerations()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(viewResult.ViewData.ContainsKey("Generations"));
        Assert.NotNull(viewResult.ViewData["Generations"]);
    }

    #endregion
}
