using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Repositories;

/// <summary>
/// Tests for ParentChild repository evidence and family context methods (Phase 4.2).
/// </summary>
public class ParentChildRepositoryEvidenceTests
{
    private RushtonRootsDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<RushtonRootsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new RushtonRootsDbContext(options);
    }

    [Fact]
    public async Task GetSourcesAsync_ReturnsSourcesLinkedThroughFactCitations()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var parent = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var child = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var relationship = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" };
        var source = new Source { Id = 1, Title = "Birth Certificate", SourceType = "Document" };
        var citation = new Citation { Id = 1, SourceId = 1, Source = source };
        var factCitation = new FactCitation 
        { 
            Id = 1, 
            EntityType = "ParentChild", 
            EntityId = 1, 
            FieldName = "RelationshipType", 
            CitationId = 1,
            Citation = citation,
            AddedByUserId = "user1"
        };

        context.People.AddRange(parent, child);
        context.ParentChildren.Add(relationship);
        context.Sources.Add(source);
        context.Citations.Add(citation);
        context.FactCitations.Add(factCitation);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetSourcesAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Birth Certificate", result[0].Title);
    }

    [Fact]
    public async Task GetSourcesAsync_ReturnsEmptyListWhenNoSourcesLinked()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var parent = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var child = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var relationship = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" };

        context.People.AddRange(parent, child);
        context.ParentChildren.Add(relationship);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetSourcesAsync(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGrandparentsAsync_ReturnsParentsOfParent()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var grandparent1 = new Person { Id = 1, FirstName = "GrandPa", LastName = "Smith" };
        var grandparent2 = new Person { Id = 2, FirstName = "GrandMa", LastName = "Smith" };
        var parent = new Person { Id = 3, FirstName = "John", LastName = "Doe" };
        var child = new Person { Id = 4, FirstName = "Jane", LastName = "Doe" };

        var grandparentRelationship1 = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" };
        var grandparentRelationship2 = new ParentChild { Id = 2, ParentPersonId = 2, ChildPersonId = 3, RelationshipType = "Biological" };
        var parentChildRelationship = new ParentChild { Id = 3, ParentPersonId = 3, ChildPersonId = 4, RelationshipType = "Biological" };

        context.People.AddRange(grandparent1, grandparent2, parent, child);
        context.ParentChildren.AddRange(grandparentRelationship1, grandparentRelationship2, parentChildRelationship);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetGrandparentsAsync(3);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.FirstName == "GrandPa");
        Assert.Contains(result, p => p.FirstName == "GrandMa");
    }

    [Fact]
    public async Task GetGrandparentsAsync_ReturnsEmptyListWhenNoGrandparents()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var parent = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var child = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var relationship = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" };

        context.People.AddRange(parent, child);
        context.ParentChildren.Add(relationship);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetGrandparentsAsync(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGrandparentsAsync_ReturnsEmptyListWhenRelationshipNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        // Act
        var result = await repository.GetGrandparentsAsync(999);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSiblingsAsync_ReturnsOtherChildrenOfSameParent()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var parent = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var child1 = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var child2 = new Person { Id = 3, FirstName = "Jack", LastName = "Doe" };
        var child3 = new Person { Id = 4, FirstName = "Jill", LastName = "Doe" };

        var relationship1 = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" };
        var relationship2 = new ParentChild { Id = 2, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" };
        var relationship3 = new ParentChild { Id = 3, ParentPersonId = 1, ChildPersonId = 4, RelationshipType = "Biological" };

        context.People.AddRange(parent, child1, child2, child3);
        context.ParentChildren.AddRange(relationship1, relationship2, relationship3);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetSiblingsAsync(1); // Jane's siblings

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.FirstName == "Jack");
        Assert.Contains(result, p => p.FirstName == "Jill");
        Assert.DoesNotContain(result, p => p.FirstName == "Jane"); // Should not include self
    }

    [Fact]
    public async Task GetSiblingsAsync_ReturnsEmptyListWhenNoSiblings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var parent = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var child = new Person { Id = 2, FirstName = "Jane", LastName = "Doe" };
        var relationship = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 2, RelationshipType = "Biological" };

        context.People.AddRange(parent, child);
        context.ParentChildren.Add(relationship);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetSiblingsAsync(1);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSiblingsAsync_ReturnsEmptyListWhenRelationshipNotFound()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        // Act
        var result = await repository.GetSiblingsAsync(999);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSiblingsAsync_HandlesMultipleSources_ReturnsDistinctSiblings()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new ParentChildRepository(context);

        var parent1 = new Person { Id = 1, FirstName = "John", LastName = "Doe" };
        var parent2 = new Person { Id = 2, FirstName = "Mary", LastName = "Doe" };
        var child1 = new Person { Id = 3, FirstName = "Jane", LastName = "Doe" };
        var child2 = new Person { Id = 4, FirstName = "Jack", LastName = "Doe" };

        // Both children have same two parents
        var relationship1 = new ParentChild { Id = 1, ParentPersonId = 1, ChildPersonId = 3, RelationshipType = "Biological" };
        var relationship2 = new ParentChild { Id = 2, ParentPersonId = 1, ChildPersonId = 4, RelationshipType = "Biological" };
        var relationship3 = new ParentChild { Id = 3, ParentPersonId = 2, ChildPersonId = 3, RelationshipType = "Biological" };
        var relationship4 = new ParentChild { Id = 4, ParentPersonId = 2, ChildPersonId = 4, RelationshipType = "Biological" };

        context.People.AddRange(parent1, parent2, child1, child2);
        context.ParentChildren.AddRange(relationship1, relationship2, relationship3, relationship4);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetSiblingsAsync(1); // Jane's siblings via parent1

        // Assert
        Assert.Single(result);
        Assert.Equal("Jack", result[0].FirstName);
    }
}
