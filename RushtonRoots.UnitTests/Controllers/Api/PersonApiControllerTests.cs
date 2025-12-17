using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Web.Controllers.Api;
using Xunit;

namespace RushtonRoots.UnitTests.Controllers.Api;

public class PersonApiControllerTests
{
    private readonly IPersonService _mockPersonService;
    private readonly ILogger<PersonApiController> _mockLogger;
    private readonly PersonApiController _controller;

    public PersonApiControllerTests()
    {
        _mockPersonService = A.Fake<IPersonService>();
        _mockLogger = A.Fake<ILogger<PersonApiController>>();
        _controller = new PersonApiController(_mockPersonService, _mockLogger);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithPersons()
    {
        // Arrange
        var persons = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new PersonViewModel { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(persons);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPersons = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(okResult.Value);
        Assert.Equal(2, returnedPersons.Count());
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var persons = new List<PersonViewModel>();
        for (int i = 1; i <= 100; i++)
        {
            persons.Add(new PersonViewModel { Id = i, FirstName = $"Person{i}", LastName = "Test" });
        }

        A.CallTo(() => _mockPersonService.GetAllAsync()).Returns(persons);

        // Act
        var result = await _controller.GetAll(page: 2, pageSize: 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPersons = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(okResult.Value);
        Assert.Equal(10, returnedPersons.Count());
        Assert.Equal(11, returnedPersons.First().Id); // Second page starts at item 11
    }

    [Fact]
    public async Task GetAll_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetAllAsync()).Throws<Exception>();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkWithPerson()
    {
        // Arrange
        var personId = 1;
        var person = new PersonViewModel { Id = personId, FirstName = "John", LastName = "Doe" };

        A.CallTo(() => _mockPersonService.GetByIdAsync(personId)).Returns(person);

        // Act
        var result = await _controller.GetById(personId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPerson = Assert.IsType<PersonViewModel>(okResult.Value);
        Assert.Equal(personId, returnedPerson.Id);
        Assert.Equal("John", returnedPerson.FirstName);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var personId = 999;
        A.CallTo(() => _mockPersonService.GetByIdAsync(personId)).Returns((PersonViewModel?)null);

        // Act
        var result = await _controller.GetById(personId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Contains("not found", notFoundResult.Value?.ToString());
    }

    [Fact]
    public async Task GetById_WhenServiceThrows_Returns500()
    {
        // Arrange
        A.CallTo(() => _mockPersonService.GetByIdAsync(A<int>._)).Throws<Exception>();

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Search Tests

    [Fact]
    public async Task Search_WithValidRequest_ReturnsOkWithResults()
    {
        // Arrange
        var searchRequest = new SearchPersonRequest { SearchTerm = "John" };
        var searchResults = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe" },
            new PersonViewModel { Id = 2, FirstName = "Johnny", LastName = "Smith" }
        };

        A.CallTo(() => _mockPersonService.SearchAsync(searchRequest)).Returns(searchResults);

        // Act
        var result = await _controller.Search(searchRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedPersons = Assert.IsAssignableFrom<IEnumerable<PersonViewModel>>(okResult.Value);
        Assert.Equal(2, returnedPersons.Count());
    }

    [Fact]
    public async Task Search_WhenServiceThrows_Returns500()
    {
        // Arrange
        var searchRequest = new SearchPersonRequest { SearchTerm = "Test" };
        A.CallTo(() => _mockPersonService.SearchAsync(A<SearchPersonRequest>._)).Throws<Exception>();

        // Act
        var result = await _controller.Search(searchRequest);

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
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1
        };

        var createdPerson = new PersonViewModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1
        };

        A.CallTo(() => _mockPersonService.CreateAsync(request)).Returns(createdPerson);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(PersonApiController.GetById), createdResult.ActionName);
        var returnedPerson = Assert.IsType<PersonViewModel>(createdResult.Value);
        Assert.Equal(1, returnedPerson.Id);
    }

    [Fact]
    public async Task Create_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe"
        };

        A.CallTo(() => _mockPersonService.CreateAsync(request))
            .Throws(new ValidationException("First name is required."));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = result.Result as ObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Create_WhenServiceThrows_Returns500()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe"
        };

        A.CallTo(() => _mockPersonService.CreateAsync(A<CreatePersonRequest>._)).Throws<Exception>();

        // Act
        var result = await _controller.Create(request);

        // Assert
        var statusResult = result.Result as ObjectResult;
        Assert.NotNull(statusResult);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidRequest_ReturnsOkWithUpdatedPerson()
    {
        // Arrange
        var personId = 1;
        var request = new UpdatePersonRequest
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1
        };

        var updatedPerson = new PersonViewModel
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe"
        };

        A.CallTo(() => _mockPersonService.UpdateAsync(request)).Returns(updatedPerson);

        // Act
        var result = await _controller.Update(personId, request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        var returnedPerson = Assert.IsType<PersonViewModel>(okResult.Value);
        Assert.Equal(personId, returnedPerson.Id);
    }

    [Fact]
    public async Task Update_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdatePersonRequest
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _controller.Update(2, request); // ID mismatch

        // Assert
        var badRequestResult = result.Result as ObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Update_WithNonExistentPerson_ReturnsNotFound()
    {
        // Arrange
        var personId = 999;
        var request = new UpdatePersonRequest
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe"
        };

        A.CallTo(() => _mockPersonService.UpdateAsync(request))
            .Throws(new NotFoundException($"Person with ID {personId} not found."));

        // Act
        var result = await _controller.Update(personId, request);

        // Assert
        var notFoundResult = result.Result as ObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task Update_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var personId = 1;
        var request = new UpdatePersonRequest
        {
            Id = personId,
            FirstName = "",
            LastName = "Doe"
        };

        A.CallTo(() => _mockPersonService.UpdateAsync(request))
            .Throws(new ValidationException("First name is required."));

        // Act
        var result = await _controller.Update(personId, request);

        // Assert
        var badRequestResult = result.Result as ObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Update_WhenServiceThrows_Returns500()
    {
        // Arrange
        var personId = 1;
        var request = new UpdatePersonRequest
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe"
        };

        A.CallTo(() => _mockPersonService.UpdateAsync(A<UpdatePersonRequest>._)).Throws<Exception>();

        // Act
        var result = await _controller.Update(personId, request);

        // Assert
        var statusResult = result.Result as ObjectResult;
        Assert.NotNull(statusResult);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var personId = 1;
        A.CallTo(() => _mockPersonService.DeleteAsync(personId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(personId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        A.CallTo(() => _mockPersonService.DeleteAsync(personId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Delete_WithNonExistentPerson_ReturnsNotFound()
    {
        // Arrange
        var personId = 999;
        A.CallTo(() => _mockPersonService.DeleteAsync(personId))
            .Throws(new NotFoundException($"Person with ID {personId} not found."));

        // Act
        var result = await _controller.Delete(personId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public async Task Delete_WhenServiceThrows_Returns500()
    {
        // Arrange
        var personId = 1;
        A.CallTo(() => _mockPersonService.DeleteAsync(A<int>._)).Throws<Exception>();

        // Act
        var result = await _controller.Delete(personId);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    #endregion
}
