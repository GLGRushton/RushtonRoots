using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class PartnershipControllerTests
{
    private readonly IPartnershipService _mockPartnershipService;
    private readonly ILogger<PartnershipController> _mockLogger;
    private readonly PartnershipController _controller;

    public PartnershipControllerTests()
    {
        _mockPartnershipService = A.Fake<IPartnershipService>();
        _mockLogger = A.Fake<ILogger<PartnershipController>>();
        _controller = new PartnershipController(_mockPartnershipService, _mockLogger);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithPartnerships()
    {
        // Arrange
        var partnerships = new List<PartnershipViewModel>
        {
            new PartnershipViewModel { Id = 1, PersonAId = 1, PersonBId = 2, PartnershipType = "Married" },
            new PartnershipViewModel { Id = 2, PersonAId = 3, PersonBId = 4, PartnershipType = "Partnered" }
        };

        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Returns(partnerships);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPartnerships = Assert.IsAssignableFrom<IEnumerable<PartnershipViewModel>>(okResult.Value);
        Assert.Equal(2, returnedPartnerships.Count());
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var partnerships = new List<PartnershipViewModel>();
        for (int i = 1; i <= 100; i++)
        {
            partnerships.Add(new PartnershipViewModel { Id = i, PersonAId = i, PersonBId = i + 1, PartnershipType = "Married" });
        }

        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Returns(partnerships);

        // Act
        var result = await _controller.GetAll(page: 2, pageSize: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPartnerships = Assert.IsAssignableFrom<IEnumerable<PartnershipViewModel>>(okResult.Value);
        Assert.Equal(10, returnedPartnerships.Count());
        Assert.Equal(11, returnedPartnerships.First().Id); // Second page starts at item 11
    }

    [Fact]
    public async Task GetAll_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithPartnership()
    {
        // Arrange
        var partnership = new PartnershipViewModel { Id = 1, PersonAId = 1, PersonBId = 2, PartnershipType = "Married" };
        A.CallTo(() => _mockPartnershipService.GetByIdAsync(1)).Returns(partnership);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPartnership = Assert.IsType<PartnershipViewModel>(okResult.Value);
        Assert.Equal(1, returnedPartnership.Id);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.GetByIdAsync(999)).Returns((PartnershipViewModel?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.GetByIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetByPersonId Tests

    [Fact]
    public async Task GetByPersonId_ReturnsOkWithPartnerships()
    {
        // Arrange
        var partnerships = new List<PartnershipViewModel>
        {
            new PartnershipViewModel { Id = 1, PersonAId = 1, PersonBId = 2, PartnershipType = "Married" },
            new PartnershipViewModel { Id = 2, PersonAId = 1, PersonBId = 3, PartnershipType = "Partnered" }
        };

        A.CallTo(() => _mockPartnershipService.GetByPersonIdAsync(1)).Returns(partnerships);

        // Act
        var result = await _controller.GetByPersonId(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPartnerships = Assert.IsAssignableFrom<IEnumerable<PartnershipViewModel>>(okResult.Value);
        Assert.Equal(2, returnedPartnerships.Count());
    }

    [Fact]
    public async Task GetByPersonId_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.GetByPersonIdAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.GetByPersonId(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreatePartnershipRequest
        {
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married",
            StartDate = DateTime.Now
        };

        var createdPartnership = new PartnershipViewModel
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.CreateAsync(request)).Returns(createdPartnership);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
        var returnedPartnership = Assert.IsType<PartnershipViewModel>(createdAtActionResult.Value);
        Assert.Equal(1, returnedPartnership.Id);
    }

    [Fact]
    public async Task Create_WithSelfPartnership_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePartnershipRequest
        {
            PersonAId = 1,
            PersonBId = 1, // Same person
            PartnershipType = "Married"
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("A person cannot be partnered with themselves", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePartnershipRequest
        {
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.CreateAsync(request)).Throws(new ArgumentException("Invalid partnership"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid partnership", badRequestResult.Value);
    }

    [Fact]
    public async Task Create_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        var request = new CreatePartnershipRequest
        {
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.CreateAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.Create(request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new UpdatePartnershipRequest
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        var updatedPartnership = new PartnershipViewModel
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.UpdateAsync(request)).Returns(updatedPartnership);

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPartnership = Assert.IsType<PartnershipViewModel>(okResult.Value);
        Assert.Equal(1, returnedPartnership.Id);
    }

    [Fact]
    public async Task Update_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdatePartnershipRequest
        {
            Id = 2, // Mismatched ID
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("ID in URL does not match ID in request body", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_WithSelfPartnership_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdatePartnershipRequest
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 1, // Same person
            PartnershipType = "Married"
        };

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("A person cannot be partnered with themselves", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdatePartnershipRequest
        {
            Id = 999,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.UpdateAsync(request)).Throws(new KeyNotFoundException("Partnership not found"));

        // Act
        var result = await _controller.Update(999, request);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenServiceThrowsArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdatePartnershipRequest
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.UpdateAsync(request)).Throws(new ArgumentException("Invalid update"));

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Invalid update", badRequestResult.Value);
    }

    [Fact]
    public async Task Update_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        var request = new UpdatePartnershipRequest
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            PartnershipType = "Married"
        };

        A.CallTo(() => _mockPartnershipService.UpdateAsync(request)).Throws<Exception>();

        // Act
        var result = await _controller.Update(1, request);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.DeleteAsync(999)).Throws(new KeyNotFoundException("Partnership not found"));

        // Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPartnershipService.DeleteAsync(1)).Throws<Exception>();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion
}
