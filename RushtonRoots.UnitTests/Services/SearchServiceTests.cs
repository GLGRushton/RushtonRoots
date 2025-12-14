using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

/// <summary>
/// Unit tests for SearchService.
/// </summary>
public class SearchServiceTests
{
    private readonly IPersonRepository _personRepository;
    private readonly IParentChildRepository _parentChildRepository;
    private readonly IPartnershipRepository _partnershipRepository;
    private readonly IPersonMapper _personMapper;
    private readonly SearchService _searchService;

    public SearchServiceTests()
    {
        _personRepository = A.Fake<IPersonRepository>();
        _parentChildRepository = A.Fake<IParentChildRepository>();
        _partnershipRepository = A.Fake<IPartnershipRepository>();
        _personMapper = A.Fake<IPersonMapper>();
        _searchService = new SearchService(
            _personRepository,
            _parentChildRepository,
            _partnershipRepository,
            _personMapper);
    }

    [Fact]
    public async Task FindRelationshipAsync_SamePerson_ReturnsSamePersonRelationship()
    {
        // Arrange
        var person = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        A.CallTo(() => _personRepository.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => _personRepository.GetAllAsync()).Returns(new List<Person> { person });

        // Act
        var result = await _searchService.FindRelationshipAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Degree);
        Assert.Equal("Same person", result.RelationshipDescription);
    }

    [Fact]
    public async Task FindRelationshipAsync_PersonNotFound_ReturnsNull()
    {
        // Arrange
        A.CallTo(() => _personRepository.GetByIdAsync(1)).Returns((Person?)null);
        A.CallTo(() => _personRepository.GetByIdAsync(2)).Returns(new Person { Id = 2 });

        // Act
        var result = await _searchService.FindRelationshipAsync(1, 2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindRelationshipAsync_ParentChild_ReturnsParentRelationship()
    {
        // Arrange
        var parent = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var child = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" };
        
        A.CallTo(() => _personRepository.GetByIdAsync(1)).Returns(parent);
        A.CallTo(() => _personRepository.GetByIdAsync(2)).Returns(child);
        A.CallTo(() => _personRepository.GetAllAsync()).Returns(new List<Person> { parent, child });
        
        var parentChildRel = new ParentChild
        {
            ParentPersonId = 1,
            ChildPersonId = 2
        };
        
        A.CallTo(() => _parentChildRepository.GetByChildIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => _parentChildRepository.GetByParentIdAsync(1)).Returns(new List<ParentChild> { parentChildRel });
        A.CallTo(() => _parentChildRepository.GetByChildIdAsync(2)).Returns(new List<ParentChild> { parentChildRel });
        A.CallTo(() => _partnershipRepository.GetByPersonIdAsync(A<int>._)).Returns(new List<Partnership>());

        // Act
        var result = await _searchService.FindRelationshipAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Degree);
        Assert.Contains("Child", result.RelationshipDescription);
    }

    [Fact]
    public async Task GetSurnameDistributionAsync_ReturnsCorrectDistribution()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person { Id = 1, FirstName = "John", LastName = "Smith", IsDeceased = false },
            new Person { Id = 2, FirstName = "Jane", LastName = "Smith", IsDeceased = true },
            new Person { Id = 3, FirstName = "Bob", LastName = "Jones", IsDeceased = false },
            new Person { Id = 4, FirstName = "Alice", LastName = "smith", IsDeceased = false }
        };

        A.CallTo(() => _personRepository.GetAllAsync()).Returns(people);

        // Act
        var result = await _searchService.GetSurnameDistributionAsync();

        // Assert
        var distribution = result.ToList();
        Assert.Equal(2, distribution.Count);
        
        var smithDist = distribution.First(d => d.Surname == "SMITH");
        Assert.Equal(3, smithDist.Count);
        Assert.Equal(2, smithDist.LivingCount);
        Assert.Equal(1, smithDist.DeceasedCount);
        
        var jonesDist = distribution.First(d => d.Surname == "JONES");
        Assert.Equal(1, jonesDist.Count);
        Assert.Equal(1, jonesDist.LivingCount);
        Assert.Equal(0, jonesDist.DeceasedCount);
    }

    [Fact]
    public async Task GetPeopleByLocationAsync_ReturnsCorrectPeople()
    {
        // Arrange
        var location1 = new Location { Id = 1, Name = "New York" };
        var person1 = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            LifeEvents = new List<LifeEvent>
            {
                new LifeEvent { LocationId = 1, Location = location1 }
            }
        };
        var person2 = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            LifeEvents = new List<LifeEvent>
            {
                new LifeEvent { LocationId = 2 }
            }
        };

        A.CallTo(() => _personRepository.GetAllAsync()).Returns(new List<Person> { person1, person2 });
        A.CallTo(() => _personMapper.MapToViewModel(person1)).Returns(new PersonViewModel { Id = 1 });

        // Act
        var result = await _searchService.GetPeopleByLocationAsync(1);

        // Assert
        var people = result.ToList();
        Assert.Single(people);
        Assert.Equal(1, people[0].Id);
    }

    [Fact]
    public async Task GetPeopleByEventTypeAsync_ReturnsCorrectPeople()
    {
        // Arrange
        var person1 = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            LifeEvents = new List<LifeEvent>
            {
                new LifeEvent { EventType = "Education" }
            }
        };
        var person2 = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            LifeEvents = new List<LifeEvent>
            {
                new LifeEvent { EventType = "Career" }
            }
        };

        A.CallTo(() => _personRepository.GetAllAsync()).Returns(new List<Person> { person1, person2 });
        A.CallTo(() => _personMapper.MapToViewModel(person1)).Returns(new PersonViewModel { Id = 1 });

        // Act
        var result = await _searchService.GetPeopleByEventTypeAsync("Education");

        // Assert
        var people = result.ToList();
        Assert.Single(people);
        Assert.Equal(1, people[0].Id);
    }

    [Fact]
    public async Task GetPeopleByEventTypeAsync_CaseInsensitive_ReturnsCorrectPeople()
    {
        // Arrange
        var person1 = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            LifeEvents = new List<LifeEvent>
            {
                new LifeEvent { EventType = "EDUCATION" }
            }
        };

        A.CallTo(() => _personRepository.GetAllAsync()).Returns(new List<Person> { person1 });
        A.CallTo(() => _personMapper.MapToViewModel(person1)).Returns(new PersonViewModel { Id = 1 });

        // Act
        var result = await _searchService.GetPeopleByEventTypeAsync("education");

        // Assert
        var people = result.ToList();
        Assert.Single(people);
    }
}
