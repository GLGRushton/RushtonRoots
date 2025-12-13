using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Application.Validators;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class PersonServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsPersonViewModel_WhenPersonExists()
    {
        // Arrange
        var personId = 1;
        var person = new Person
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1,
            Household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 }
        };

        var expectedViewModel = new PersonViewModel
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1,
            HouseholdName = "Test Household"
        };

        var mockRepository = A.Fake<IPersonRepository>();
        var mockMapper = A.Fake<IPersonMapper>();
        var mockValidator = A.Fake<IPersonValidator>();

        A.CallTo(() => mockRepository.GetByIdAsync(personId)).Returns(person);
        A.CallTo(() => mockMapper.MapToViewModel(person)).Returns(expectedViewModel);

        var service = new PersonService(mockRepository, mockMapper, mockValidator);

        // Act
        var result = await service.GetByIdAsync(personId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Id, result.Id);
        Assert.Equal(expectedViewModel.FirstName, result.FirstName);
        Assert.Equal(expectedViewModel.LastName, result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenPersonDoesNotExist()
    {
        // Arrange
        var personId = 999;

        var mockRepository = A.Fake<IPersonRepository>();
        var mockMapper = A.Fake<IPersonMapper>();
        var mockValidator = A.Fake<IPersonValidator>();

        A.CallTo(() => mockRepository.GetByIdAsync(personId)).Returns((Person?)null);

        var service = new PersonService(mockRepository, mockMapper, mockValidator);

        // Act
        var result = await service.GetByIdAsync(personId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidationException_WhenValidationFails()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "",
            LastName = "Doe",
            HouseholdId = 1
        };

        var validationResult = new ValidationResult
        {
            IsValid = false,
            Errors = new List<string> { "First name is required." }
        };

        var mockRepository = A.Fake<IPersonRepository>();
        var mockMapper = A.Fake<IPersonMapper>();
        var mockValidator = A.Fake<IPersonValidator>();

        A.CallTo(() => mockValidator.ValidateCreateAsync(request)).Returns(validationResult);

        var service = new PersonService(mockRepository, mockMapper, mockValidator);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsPersonViewModel_WhenValidationPasses()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1
        };

        var person = new Person
        {
            Id = 0,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1
        };

        var savedPerson = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1,
            Household = new Household { Id = 1, HouseholdName = "Test Household", AnchorPersonId = 1 }
        };

        var expectedViewModel = new PersonViewModel
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            HouseholdId = 1,
            HouseholdName = "Test Household"
        };

        var validationResult = new ValidationResult { IsValid = true };

        var mockRepository = A.Fake<IPersonRepository>();
        var mockMapper = A.Fake<IPersonMapper>();
        var mockValidator = A.Fake<IPersonValidator>();

        A.CallTo(() => mockValidator.ValidateCreateAsync(request)).Returns(validationResult);
        A.CallTo(() => mockMapper.MapToEntity(request)).Returns(person);
        A.CallTo(() => mockRepository.AddAsync(person)).Returns(savedPerson);
        A.CallTo(() => mockRepository.GetByIdAsync(1)).Returns(savedPerson);
        A.CallTo(() => mockMapper.MapToViewModel(savedPerson)).Returns(expectedViewModel);

        var service = new PersonService(mockRepository, mockMapper, mockValidator);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Id, result.Id);
        Assert.Equal(expectedViewModel.FirstName, result.FirstName);
    }

    [Fact]
    public async Task SearchAsync_ReturnsFilteredPeople()
    {
        // Arrange
        var searchRequest = new SearchPersonRequest
        {
            SearchTerm = "John",
            IsDeceased = false
        };

        var people = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1, IsDeceased = false }
        };

        var viewModels = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 1, FirstName = "John", LastName = "Doe", HouseholdId = 1 }
        };

        var mockRepository = A.Fake<IPersonRepository>();
        var mockMapper = A.Fake<IPersonMapper>();
        var mockValidator = A.Fake<IPersonValidator>();

        A.CallTo(() => mockRepository.SearchAsync(searchRequest)).Returns(people);
        A.CallTo(() => mockMapper.MapToViewModel(A<Person>._)).ReturnsLazily((Person p) => viewModels.First());

        var service = new PersonService(mockRepository, mockMapper, mockValidator);

        // Act
        var result = await service.SearchAsync(searchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }
}
