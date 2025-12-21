using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;
using Xunit;

namespace RushtonRoots.UnitTests.Mappers;

public class ParentChildMapperTests
{
    private readonly ParentChildMapper _mapper;

    public ParentChildMapperTests()
    {
        _mapper = new ParentChildMapper();
    }

    #region MapToViewModel Tests

    [Fact]
    public void MapToViewModel_WithCompleteData_MapsAllFields()
    {
        // Arrange
        var parent = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1970, 1, 1),
            DateOfDeath = new DateTime(2020, 1, 1),
            PhotoUrl = "http://example.com/john.jpg"
        };

        var child = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = new DateTime(2000, 6, 15),
            DateOfDeath = new DateTime(2021, 6, 15),
            IsDeceased = true,
            PhotoUrl = "http://example.com/jane.jpg"
        };

        var parentChild = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            ParentPerson = parent,
            ChildPerson = child,
            RelationshipType = "Biological",
            Notes = "Test notes",
            ConfidenceScore = 95,
            IsVerified = true,
            VerifiedDate = new DateTime(2023, 5, 1),
            VerifiedBy = "admin@test.com",
            CreatedDateTime = new DateTime(2023, 1, 1),
            UpdatedDateTime = new DateTime(2023, 6, 1)
        };

        // Act
        var result = _mapper.MapToViewModel(parentChild);

        // Assert
        Assert.Equal(10, result.Id);
        Assert.Equal(1, result.ParentPersonId);
        Assert.Equal(2, result.ChildPersonId);
        Assert.Equal("John Doe", result.ParentName);
        Assert.Equal("Jane Doe", result.ChildName);
        Assert.Equal("http://example.com/john.jpg", result.ParentPhotoUrl);
        Assert.Equal("http://example.com/jane.jpg", result.ChildPhotoUrl);
        Assert.Equal(new DateTime(1970, 1, 1), result.ParentBirthDate);
        Assert.Equal(new DateTime(2020, 1, 1), result.ParentDeathDate);
        Assert.Equal(new DateTime(2000, 6, 15), result.ChildBirthDate);
        Assert.Equal(new DateTime(2021, 6, 15), result.ChildDeathDate);
        Assert.Equal(21, result.ChildAge); // Age at death
        Assert.Equal("Biological", result.RelationshipType);
        Assert.Equal("Test notes", result.Notes);
        Assert.Equal(95, result.ConfidenceScore);
        Assert.True(result.IsVerified);
        Assert.Equal(new DateTime(2023, 5, 1), result.VerifiedDate);
        Assert.Equal("admin@test.com", result.VerifiedBy);
        Assert.Equal(new DateTime(2023, 1, 1), result.CreatedDateTime);
        Assert.Equal(new DateTime(2023, 6, 1), result.UpdatedDateTime);
    }

    [Fact]
    public void MapToViewModel_WithLivingChild_CalculatesCurrentAge()
    {
        // Arrange
        var child = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = DateTime.Today.AddYears(-10), // 10 years old
            IsDeceased = false
        };

        var parentChild = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            ChildPerson = child,
            RelationshipType = "Biological",
            CreatedDateTime = DateTime.Now,
            UpdatedDateTime = DateTime.Now
        };

        // Act
        var result = _mapper.MapToViewModel(parentChild);

        // Assert
        Assert.Equal(10, result.ChildAge);
    }

    [Fact]
    public void MapToViewModel_WithDeceasedChild_CalculatesAgeAtDeath()
    {
        // Arrange
        var birthDate = new DateTime(2000, 6, 15);
        var deathDate = new DateTime(2021, 8, 20);

        var child = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = birthDate,
            DateOfDeath = deathDate,
            IsDeceased = true
        };

        var parentChild = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            ChildPerson = child,
            RelationshipType = "Biological",
            CreatedDateTime = DateTime.Now,
            UpdatedDateTime = DateTime.Now
        };

        // Act
        var result = _mapper.MapToViewModel(parentChild);

        // Assert
        Assert.Equal(21, result.ChildAge); // Died at 21
    }

    [Fact]
    public void MapToViewModel_WithNullPersons_ReturnsUnknownNames()
    {
        // Arrange
        var parentChild = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            ParentPerson = null,
            ChildPerson = null,
            RelationshipType = "Biological",
            CreatedDateTime = DateTime.Now,
            UpdatedDateTime = DateTime.Now
        };

        // Act
        var result = _mapper.MapToViewModel(parentChild);

        // Assert
        Assert.Equal("Unknown", result.ParentName);
        Assert.Equal("Unknown", result.ChildName);
        Assert.Null(result.ParentPhotoUrl);
        Assert.Null(result.ChildPhotoUrl);
        Assert.Null(result.ChildAge);
    }

    [Fact]
    public void MapToViewModel_WithNullOptionalFields_HandlesGracefully()
    {
        // Arrange
        var parentChild = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            ParentPerson = new Person { Id = 1, FirstName = "John", LastName = "Doe" },
            ChildPerson = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" },
            RelationshipType = "Biological",
            Notes = null,
            ConfidenceScore = null,
            CreatedDateTime = DateTime.Now,
            UpdatedDateTime = DateTime.Now
        };

        // Act
        var result = _mapper.MapToViewModel(parentChild);

        // Assert
        Assert.Null(result.Notes);
        Assert.Null(result.ConfidenceScore);
        Assert.Null(result.ParentBirthDate);
        Assert.Null(result.ParentDeathDate);
        Assert.Null(result.ChildBirthDate);
        Assert.Null(result.ChildDeathDate);
    }

    [Fact]
    public void MapToViewModel_WithBirthdayNotYetOccurredThisYear_AdjustsAge()
    {
        // Arrange
        var today = DateTime.Today;
        var birthDate = today.AddYears(-10).AddDays(1); // Birthday is tomorrow

        var child = new Person
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Doe",
            DateOfBirth = birthDate,
            IsDeceased = false
        };

        var parentChild = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            ChildPerson = child,
            RelationshipType = "Biological",
            CreatedDateTime = DateTime.Now,
            UpdatedDateTime = DateTime.Now
        };

        // Act
        var result = _mapper.MapToViewModel(parentChild);

        // Assert
        Assert.Equal(9, result.ChildAge); // Still 9 until birthday
    }

    #endregion

    #region MapToEntity Tests

    [Fact]
    public void MapToEntity_WithValidRequest_CreatesEntity()
    {
        // Arrange
        var request = new CreateParentChildRequest
        {
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        // Act
        var result = _mapper.MapToEntity(request);

        // Assert
        Assert.Equal(1, result.ParentPersonId);
        Assert.Equal(2, result.ChildPersonId);
        Assert.Equal("Biological", result.RelationshipType);
        Assert.Equal(0, result.Id); // Not set yet
    }

    #endregion

    #region UpdateEntity Tests

    [Fact]
    public void UpdateEntity_WithValidRequest_UpdatesEntity()
    {
        // Arrange
        var entity = new ParentChild
        {
            Id = 10,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = "Old notes",
            ConfidenceScore = 80
        };

        var request = new UpdateParentChildRequest
        {
            Id = 10,
            ParentPersonId = 3,
            ChildPersonId = 4,
            RelationshipType = "Adopted"
        };

        // Act
        _mapper.UpdateEntity(entity, request);

        // Assert
        Assert.Equal(10, entity.Id); // ID should not change
        Assert.Equal(3, entity.ParentPersonId);
        Assert.Equal(4, entity.ChildPersonId);
        Assert.Equal("Adopted", entity.RelationshipType);
        // Notes and ConfidenceScore should remain unchanged
        Assert.Equal("Old notes", entity.Notes);
        Assert.Equal(80, entity.ConfidenceScore);
    }

    #endregion
}
