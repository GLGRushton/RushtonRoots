using FakeItEasy;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class FamilyTreeServiceTests
{
    [Fact]
    public async Task GetMiniTreeAsync_WithValidPersonId_ReturnsTreeNode()
    {
        // Arrange
        var mockPersonRepository = A.Fake<IPersonRepository>();
        var mockParentChildRepository = A.Fake<IParentChildRepository>();
        var mockPartnershipRepository = A.Fake<IPartnershipRepository>();

        var person = new Person
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1980, 1, 1)
        };

        A.CallTo(() => mockPersonRepository.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => mockParentChildRepository.GetByChildIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockParentChildRepository.GetByParentIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockPartnershipRepository.GetByPersonIdAsync(1)).Returns(new List<Partnership>());

        // Note: UserManager can't be easily mocked, so we test methods that don't use it
        var service = new FamilyTreeService(
            mockPersonRepository,
            mockParentChildRepository,
            mockPartnershipRepository,
            null!); // UserManager not needed for GetMiniTreeAsync

        // Act
        var result = await service.GetMiniTreeAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal(0, result.Generation);
        Assert.NotNull(result.Parents);
        Assert.NotNull(result.Children);
        Assert.NotNull(result.Spouses);
    }

    [Fact]
    public async Task GetMiniTreeAsync_WithInvalidPersonId_ReturnsNull()
    {
        // Arrange
        var mockPersonRepository = A.Fake<IPersonRepository>();
        var mockParentChildRepository = A.Fake<IParentChildRepository>();
        var mockPartnershipRepository = A.Fake<IPartnershipRepository>();

        A.CallTo(() => mockPersonRepository.GetByIdAsync(999)).Returns((Person?)null);

        var service = new FamilyTreeService(
            mockPersonRepository,
            mockParentChildRepository,
            mockPartnershipRepository,
            null!);

        // Act
        var result = await service.GetMiniTreeAsync(999, 2);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetMiniTreeAsync_IncludesParents_WhenGenerationsGreaterThanZero()
    {
        // Arrange
        var mockPersonRepository = A.Fake<IPersonRepository>();
        var mockParentChildRepository = A.Fake<IParentChildRepository>();
        var mockPartnershipRepository = A.Fake<IPartnershipRepository>();

        var person = new Person { Id = 1, FirstName = "Child", LastName = "Doe" };
        var parent = new Person { Id = 2, FirstName = "Parent", LastName = "Doe" };
        var parentRelationship = new ParentChild
        {
            Id = 1,
            ParentPersonId = 2,
            ChildPersonId = 1,
            IsDeleted = false
        };

        A.CallTo(() => mockPersonRepository.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => mockPersonRepository.GetByIdAsync(2)).Returns(parent);
        A.CallTo(() => mockParentChildRepository.GetByChildIdAsync(1)).Returns(new List<ParentChild> { parentRelationship });
        A.CallTo(() => mockParentChildRepository.GetByChildIdAsync(2)).Returns(new List<ParentChild>());
        A.CallTo(() => mockParentChildRepository.GetByParentIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockParentChildRepository.GetByParentIdAsync(2)).Returns(new List<ParentChild>());
        A.CallTo(() => mockPartnershipRepository.GetByPersonIdAsync(1)).Returns(new List<Partnership>());
        A.CallTo(() => mockPartnershipRepository.GetByPersonIdAsync(2)).Returns(new List<Partnership>());

        var service = new FamilyTreeService(
            mockPersonRepository,
            mockParentChildRepository,
            mockPartnershipRepository,
            null!);

        // Act
        var result = await service.GetMiniTreeAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Parents);
        Assert.Single(result.Parents);
        Assert.Equal(2, result.Parents.First().Id);
        Assert.Equal("Parent Doe", result.Parents.First().Name);
        Assert.Equal(-1, result.Parents.First().Generation);
    }

    [Fact]
    public async Task GetMiniTreeAsync_IncludesChildren_Always()
    {
        // Arrange
        var mockPersonRepository = A.Fake<IPersonRepository>();
        var mockParentChildRepository = A.Fake<IParentChildRepository>();
        var mockPartnershipRepository = A.Fake<IPartnershipRepository>();

        var person = new Person { Id = 1, FirstName = "Parent", LastName = "Doe" };
        var child = new Person { Id = 2, FirstName = "Child", LastName = "Doe" };
        var childRelationship = new ParentChild
        {
            Id = 1,
            ParentPersonId = 1,
            ChildPersonId = 2,
            IsDeleted = false
        };

        A.CallTo(() => mockPersonRepository.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => mockPersonRepository.GetByIdAsync(2)).Returns(child);
        A.CallTo(() => mockParentChildRepository.GetByChildIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockParentChildRepository.GetByParentIdAsync(1)).Returns(new List<ParentChild> { childRelationship });
        A.CallTo(() => mockPartnershipRepository.GetByPersonIdAsync(1)).Returns(new List<Partnership>());

        var service = new FamilyTreeService(
            mockPersonRepository,
            mockParentChildRepository,
            mockPartnershipRepository,
            null!);

        // Act
        var result = await service.GetMiniTreeAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Children);
        Assert.Single(result.Children);
        Assert.Equal(2, result.Children.First().Id);
        Assert.Equal("Child Doe", result.Children.First().Name);
        Assert.Equal(1, result.Children.First().Generation);
    }

    [Fact]
    public async Task GetMiniTreeAsync_IncludesSpouses()
    {
        // Arrange
        var mockPersonRepository = A.Fake<IPersonRepository>();
        var mockParentChildRepository = A.Fake<IParentChildRepository>();
        var mockPartnershipRepository = A.Fake<IPartnershipRepository>();

        var person = new Person { Id = 1, FirstName = "Person", LastName = "One" };
        var spouse = new Person { Id = 2, FirstName = "Person", LastName = "Two" };
        var partnership = new Partnership
        {
            Id = 1,
            PersonAId = 1,
            PersonBId = 2,
            IsDeleted = false
        };

        A.CallTo(() => mockPersonRepository.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => mockPersonRepository.GetByIdAsync(2)).Returns(spouse);
        A.CallTo(() => mockParentChildRepository.GetByChildIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockParentChildRepository.GetByParentIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockPartnershipRepository.GetByPersonIdAsync(1)).Returns(new List<Partnership> { partnership });

        var service = new FamilyTreeService(
            mockPersonRepository,
            mockParentChildRepository,
            mockPartnershipRepository,
            null!);

        // Act
        var result = await service.GetMiniTreeAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Spouses);
        Assert.Single(result.Spouses);
        Assert.Equal(2, result.Spouses.First().Id);
        Assert.Equal("Person Two", result.Spouses.First().Name);
        Assert.Equal(0, result.Spouses.First().Generation);
    }

    [Fact]
    public async Task GetMiniTreeAsync_FiltersDeletedRelationships()
    {
        // Arrange
        var mockPersonRepository = A.Fake<IPersonRepository>();
        var mockParentChildRepository = A.Fake<IParentChildRepository>();
        var mockPartnershipRepository = A.Fake<IPartnershipRepository>();

        var person = new Person { Id = 1, FirstName = "Person", LastName = "Doe" };
        var deletedParent = new Person { Id = 2, FirstName = "Deleted", LastName = "Parent", IsDeleted = true };
        var parentRelationship = new ParentChild
        {
            Id = 1,
            ParentPersonId = 2,
            ChildPersonId = 1,
            IsDeleted = true
        };

        A.CallTo(() => mockPersonRepository.GetByIdAsync(1)).Returns(person);
        A.CallTo(() => mockPersonRepository.GetByIdAsync(2)).Returns(deletedParent);
        A.CallTo(() => mockParentChildRepository.GetByChildIdAsync(1)).Returns(new List<ParentChild> { parentRelationship });
        A.CallTo(() => mockParentChildRepository.GetByParentIdAsync(1)).Returns(new List<ParentChild>());
        A.CallTo(() => mockPartnershipRepository.GetByPersonIdAsync(1)).Returns(new List<Partnership>());

        var service = new FamilyTreeService(
            mockPersonRepository,
            mockParentChildRepository,
            mockPartnershipRepository,
            null!);

        // Act
        var result = await service.GetMiniTreeAsync(1, 2);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Parents);
        Assert.Empty(result.Parents); // Deleted relationships should be filtered out
    }
}
