using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Repositories;

/// <summary>
/// Tests for PersonRepository search functionality
/// Specifically tests the fixes for person page bugs
/// </summary>
public class PersonRepositorySearchTests : IDisposable
{
    private readonly RushtonRootsDbContext _context;
    private readonly PersonRepository _repository;

    public PersonRepositorySearchTests()
    {
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RushtonRootsDbContext(options);
        _repository = new PersonRepository(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var household1 = new Household
        {
            Id = 1,
            HouseholdName = "Smith Family",
            AnchorPersonId = 1
        };

        var household2 = new Household
        {
            Id = 2,
            HouseholdName = "Jones Family",
            AnchorPersonId = 3
        };

        _context.Households.AddRange(household1, household2);

        var people = new List<Person>
        {
            new Person
            {
                Id = 1,
                FirstName = "John",
                LastName = "Smith",
                HouseholdId = 1,
                IsDeceased = false,
                DateOfBirth = new DateTime(1980, 1, 1)
            },
            new Person
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                HouseholdId = 1,
                IsDeceased = false,
                DateOfBirth = new DateTime(1982, 5, 15)
            },
            new Person
            {
                Id = 3,
                FirstName = "Bob",
                LastName = "Jones",
                HouseholdId = 2,
                IsDeceased = true,
                DateOfBirth = new DateTime(1950, 3, 20),
                DateOfDeath = new DateTime(2020, 7, 10)
            },
            new Person
            {
                Id = 4,
                FirstName = "Alice",
                LastName = "Jones",
                HouseholdId = 2,
                IsDeceased = false,
                DateOfBirth = new DateTime(1955, 8, 5)
            },
            new Person
            {
                Id = 5,
                FirstName = "Charlie",
                LastName = "Brown",
                HouseholdId = 0, // No household assigned
                IsDeceased = false,
                DateOfBirth = new DateTime(1990, 12, 25)
            }
        };

        _context.People.AddRange(people);
        _context.SaveChanges();
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_FiltersCorrectly()
    {
        // Arrange
        var request = new SearchPersonRequest
        {
            SearchTerm = "Smith"
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, p => Assert.Contains("Smith", p.LastName));
    }

    [Fact]
    public async Task SearchAsync_WithHouseholdId_FiltersCorrectly()
    {
        // Arrange - Bug #4 fix: Filter by household should work properly
        var request = new SearchPersonRequest
        {
            HouseholdId = 1
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, p => Assert.Equal(1, p.HouseholdId));
    }

    [Fact]
    public async Task SearchAsync_WithIsDeceased_FiltersCorrectly()
    {
        // Arrange
        var request = new SearchPersonRequest
        {
            IsDeceased = true
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.True(resultList[0].IsDeceased);
        Assert.Equal("Bob", resultList[0].FirstName);
    }

    [Fact]
    public async Task SearchAsync_WithSurname_FiltersCorrectly()
    {
        // Arrange
        var request = new SearchPersonRequest
        {
            Surname = "Jones"
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, p => Assert.Equal("Jones", p.LastName));
    }

    [Fact]
    public async Task SearchAsync_WithBirthDateRange_FiltersCorrectly()
    {
        // Arrange
        var request = new SearchPersonRequest
        {
            BirthDateFrom = new DateTime(1980, 1, 1),
            BirthDateTo = new DateTime(1985, 12, 31)
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(2, resultList.Count); // John and Jane Smith
        Assert.Contains(resultList, p => p.FirstName == "John");
        Assert.Contains(resultList, p => p.FirstName == "Jane");
    }

    [Fact]
    public async Task SearchAsync_WithDeathDateRange_FiltersCorrectly()
    {
        // Arrange
        var request = new SearchPersonRequest
        {
            DeathDateFrom = new DateTime(2020, 1, 1),
            DeathDateTo = new DateTime(2020, 12, 31)
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Equal("Bob", resultList[0].FirstName);
        Assert.True(resultList[0].IsDeceased);
    }

    [Fact]
    public async Task SearchAsync_WithMultipleFilters_CombinesCorrectly()
    {
        // Arrange - Bug #4 fix: Multiple filters should work together
        var request = new SearchPersonRequest
        {
            HouseholdId = 1,
            IsDeceased = false,
            SearchTerm = "Jane"
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Single(resultList);
        Assert.Equal("Jane", resultList[0].FirstName);
        Assert.Equal("Smith", resultList[0].LastName);
        Assert.Equal(1, resultList[0].HouseholdId);
        Assert.False(resultList[0].IsDeceased);
    }

    [Fact]
    public async Task SearchAsync_WithNoFilters_ReturnsAll()
    {
        // Arrange
        var request = new SearchPersonRequest();

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.Equal(4, resultList.Count); // 4 people with valid households in test data
    }

    [Fact]
    public async Task SearchAsync_ReturnsResultsWithHouseholdNavigation()
    {
        // Arrange - Bug #5 fix: Households should be included in results
        var request = new SearchPersonRequest
        {
            HouseholdId = 1
        };

        // Act
        var results = await _repository.SearchAsync(request);

        // Assert
        var resultList = results.ToList();
        Assert.All(resultList, p =>
        {
            Assert.NotNull(p.Household);
            Assert.Equal("Smith Family", p.Household!.HouseholdName);
        });
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
