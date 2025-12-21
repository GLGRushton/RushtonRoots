using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Repositories;

/// <summary>
/// Tests for household delete impact calculation repository methods.
/// These are integration tests that use an in-memory database.
/// </summary>
public class HouseholdRepositoryDeleteImpactTests : IDisposable
{
    private readonly RushtonRootsDbContext _context;
    private readonly HouseholdRepository _repository;

    public HouseholdRepositoryDeleteImpactTests()
    {
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new RushtonRootsDbContext(options);
        _repository = new HouseholdRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetPhotoCountAsync Tests

    [Fact]
    public async Task GetPhotoCountAsync_WithNoMembers_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPhotoCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetPhotoCountAsync_WithMembersButNoPhotos_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPhotoCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetPhotoCountAsync_WithMembersAndPhotos_ReturnsCorrectCount()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        var photo1 = new PersonPhoto { Id = 1, PersonId = 1, PhotoUrl = "photo1.jpg" };
        var photo2 = new PersonPhoto { Id = 2, PersonId = 1, PhotoUrl = "photo2.jpg" };
        var photo3 = new PersonPhoto { Id = 3, PersonId = 2, PhotoUrl = "photo3.jpg" };
        _context.PersonPhotos.AddRange(photo1, photo2, photo3);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPhotoCountAsync(1);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetPhotoCountAsync_WithMultipleHouseholds_OnlyCountsCorrectHousehold()
    {
        // Arrange
        var household1 = new Household { Id = 1, HouseholdName = "Household 1" };
        var household2 = new Household { Id = 2, HouseholdName = "Household 2" };
        _context.Households.AddRange(household1, household2);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 2, FirstName = "Jane", LastName = "Smith" };
        _context.People.AddRange(person1, person2);

        var photo1 = new PersonPhoto { Id = 1, PersonId = 1, PhotoUrl = "photo1.jpg" };
        var photo2 = new PersonPhoto { Id = 2, PersonId = 1, PhotoUrl = "photo2.jpg" };
        var photo3 = new PersonPhoto { Id = 3, PersonId = 2, PhotoUrl = "photo3.jpg" };
        _context.PersonPhotos.AddRange(photo1, photo2, photo3);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPhotoCountAsync(1);

        // Assert
        Assert.Equal(2, result);
    }

    #endregion

    #region GetDocumentCountAsync Tests

    [Fact]
    public async Task GetDocumentCountAsync_WithNoMembers_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDocumentCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetDocumentCountAsync_WithMembersButNoDocuments_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDocumentCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetDocumentCountAsync_WithMembersAndDocuments_ReturnsCorrectCount()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        var doc1 = new Document { Id = 1, Title = "Doc 1", DocumentUrl = "doc1.pdf", UploadedByUserId = "user1" };
        var doc2 = new Document { Id = 2, Title = "Doc 2", DocumentUrl = "doc2.pdf", UploadedByUserId = "user1" };
        var doc3 = new Document { Id = 3, Title = "Doc 3", DocumentUrl = "doc3.pdf", UploadedByUserId = "user1" };
        _context.Documents.AddRange(doc1, doc2, doc3);

        var docPerson1 = new DocumentPerson { DocumentId = 1, PersonId = 1 };
        var docPerson2 = new DocumentPerson { DocumentId = 2, PersonId = 1 };
        var docPerson3 = new DocumentPerson { DocumentId = 3, PersonId = 2 };
        _context.DocumentPeople.AddRange(docPerson1, docPerson2, docPerson3);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDocumentCountAsync(1);

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetDocumentCountAsync_WithSharedDocument_CountsOnce()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        var doc1 = new Document { Id = 1, Title = "Shared Doc", DocumentUrl = "doc1.pdf", UploadedByUserId = "user1" };
        _context.Documents.Add(doc1);

        // Same document associated with both people
        var docPerson1 = new DocumentPerson { DocumentId = 1, PersonId = 1 };
        var docPerson2 = new DocumentPerson { DocumentId = 1, PersonId = 2 };
        _context.DocumentPeople.AddRange(docPerson1, docPerson2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetDocumentCountAsync(1);

        // Assert - Should count the document only once despite being linked to 2 people
        Assert.Equal(1, result);
    }

    #endregion

    #region GetRelationshipCountAsync Tests

    [Fact]
    public async Task GetRelationshipCountAsync_WithNoMembers_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRelationshipCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetRelationshipCountAsync_WithMembersButNoRelationships_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRelationshipCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetRelationshipCountAsync_WithPartnerships_ReturnsCorrectCount()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        _context.People.AddRange(person1, person2);

        var partnership = new Partnership { Id = 1, PersonAId = 1, PersonBId = 2, PartnershipType = "Married" };
        _context.Partnerships.Add(partnership);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRelationshipCountAsync(1);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetRelationshipCountAsync_WithParentChildRelationships_ReturnsCorrectCount()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        var person3 = new Person { Id = 3, HouseholdId = 1, FirstName = "Junior", LastName = "Doe" };
        _context.People.AddRange(person1, person2, person3);

        var parentChild1 = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" };
        var parentChild2 = new ParentChild { Id = 2, ParentPersonId = 2, ChildPersonId = 3, RelationshipType = "Biological" };
        _context.ParentChildren.AddRange(parentChild1, parentChild2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRelationshipCountAsync(1);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetRelationshipCountAsync_WithBothTypes_ReturnsCombinedCount()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 1, FirstName = "Jane", LastName = "Doe" };
        var person3 = new Person { Id = 3, HouseholdId = 1, FirstName = "Junior", LastName = "Doe" };
        _context.People.AddRange(person1, person2, person3);

        var partnership = new Partnership { Id = 1, PersonAId = 1, PersonBId = 2, PartnershipType = "Married" };
        _context.Partnerships.Add(partnership);

        var parentChild1 = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" };
        var parentChild2 = new ParentChild { Id = 2, ParentPersonId = 2, ChildPersonId = 3, RelationshipType = "Biological" };
        _context.ParentChildren.AddRange(parentChild1, parentChild2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRelationshipCountAsync(1);

        // Assert - 1 partnership + 2 parent-child = 3
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task GetRelationshipCountAsync_WithCrossHouseholdRelationship_CountsRelationship()
    {
        // Arrange
        var household1 = new Household { Id = 1, HouseholdName = "Household 1" };
        var household2 = new Household { Id = 2, HouseholdName = "Household 2" };
        _context.Households.AddRange(household1, household2);

        var person1 = new Person { Id = 1, HouseholdId = 1, FirstName = "John", LastName = "Doe" };
        var person2 = new Person { Id = 2, HouseholdId = 2, FirstName = "Jane", LastName = "Smith" };
        _context.People.AddRange(person1, person2);

        // Partnership across households
        var partnership = new Partnership { Id = 1, PersonAId = 1, PersonBId = 2, PartnershipType = "Married" };
        _context.Partnerships.Add(partnership);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRelationshipCountAsync(1);

        // Assert - Should count because person1 is in household1
        Assert.Equal(1, result);
    }

    #endregion

    #region GetEventCountAsync Tests

    [Fact]
    public async Task GetEventCountAsync_WithNoEvents_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetEventCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetEventCountAsync_WithEvents_ReturnsCorrectCount()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        var event1 = new FamilyEvent 
        { 
            Id = 1, 
            HouseholdId = 1, 
            Title = "Reunion", 
            StartDateTime = DateTime.Now,
            CreatedByUserId = "user1"
        };
        var event2 = new FamilyEvent 
        { 
            Id = 2, 
            HouseholdId = 1, 
            Title = "Birthday", 
            StartDateTime = DateTime.Now,
            CreatedByUserId = "user1"
        };
        _context.FamilyEvents.AddRange(event1, event2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetEventCountAsync(1);

        // Assert
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetEventCountAsync_WithMultipleHouseholds_OnlyCountsCorrectHousehold()
    {
        // Arrange
        var household1 = new Household { Id = 1, HouseholdName = "Household 1" };
        var household2 = new Household { Id = 2, HouseholdName = "Household 2" };
        _context.Households.AddRange(household1, household2);

        var event1 = new FamilyEvent 
        { 
            Id = 1, 
            HouseholdId = 1, 
            Title = "Event 1", 
            StartDateTime = DateTime.Now,
            CreatedByUserId = "user1"
        };
        var event2 = new FamilyEvent 
        { 
            Id = 2, 
            HouseholdId = 2, 
            Title = "Event 2", 
            StartDateTime = DateTime.Now,
            CreatedByUserId = "user1"
        };
        _context.FamilyEvents.AddRange(event1, event2);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetEventCountAsync(1);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetEventCountAsync_WithNullHouseholdId_ReturnsZero()
    {
        // Arrange
        var household = new Household { Id = 1, HouseholdName = "Test Household" };
        _context.Households.Add(household);

        // Event with null HouseholdId
        var event1 = new FamilyEvent 
        { 
            Id = 1, 
            HouseholdId = null, 
            Title = "General Event", 
            StartDateTime = DateTime.Now,
            CreatedByUserId = "user1"
        };
        _context.FamilyEvents.Add(event1);

        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetEventCountAsync(1);

        // Assert
        Assert.Equal(0, result);
    }

    #endregion
}
