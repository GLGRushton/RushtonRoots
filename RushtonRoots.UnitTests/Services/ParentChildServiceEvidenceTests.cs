using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

/// <summary>
/// Tests for ParentChild service evidence and family context methods (Phase 4.2).
/// </summary>
public class ParentChildServiceEvidenceTests
{
    private readonly IParentChildRepository _parentChildRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ILifeEventRepository _lifeEventRepository;
    private readonly IParentChildMapper _parentChildMapper;
    private readonly ISourceMapper _sourceMapper;
    private readonly ILifeEventMapper _lifeEventMapper;
    private readonly IPersonMapper _personMapper;
    private readonly ParentChildService _service;

    public ParentChildServiceEvidenceTests()
    {
        _parentChildRepository = A.Fake<IParentChildRepository>();
        _personRepository = A.Fake<IPersonRepository>();
        _lifeEventRepository = A.Fake<ILifeEventRepository>();
        _parentChildMapper = A.Fake<IParentChildMapper>();
        _sourceMapper = A.Fake<ISourceMapper>();
        _lifeEventMapper = A.Fake<ILifeEventMapper>();
        _personMapper = A.Fake<IPersonMapper>();

        _service = new ParentChildService(
            _parentChildRepository,
            _personRepository,
            _lifeEventRepository,
            _parentChildMapper,
            _sourceMapper,
            _lifeEventMapper,
            _personMapper);
    }

    [Fact]
    public async Task GetEvidenceAsync_ReturnsSourceViewModels()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var sources = new List<Source>
        {
            new Source { Id = 1, Title = "Birth Certificate", SourceType = "Document" },
            new Source { Id = 2, Title = "Census Record", SourceType = "Document" }
        };
        var sourceViewModels = new List<SourceViewModel>
        {
            new SourceViewModel { Id = 1, Title = "Birth Certificate", SourceType = "Document" },
            new SourceViewModel { Id = 2, Title = "Census Record", SourceType = "Document" }
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _parentChildRepository.GetSourcesAsync(relationshipId)).Returns(sources);
        A.CallTo(() => _sourceMapper.MapToViewModels(sources)).Returns(sourceViewModels);

        // Act
        var result = await _service.GetEvidenceAsync(relationshipId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Title == "Birth Certificate");
        Assert.Contains(result, s => s.Title == "Census Record");
    }

    [Fact]
    public async Task GetEvidenceAsync_ThrowsNotFoundException_WhenRelationshipNotFound()
    {
        // Arrange
        var relationshipId = 999;
        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns((ParentChild?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetEvidenceAsync(relationshipId));
    }

    [Fact]
    public async Task GetRelatedEventsAsync_ReturnsCombinedEventsForParentAndChild()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var parentEvents = new List<LifeEvent>
        {
            new LifeEvent { Id = 1, PersonId = 1, EventType = "Birth", Title = "Parent Birth" }
        };
        var childEvents = new List<LifeEvent>
        {
            new LifeEvent { Id = 2, PersonId = 2, EventType = "Birth", Title = "Child Birth" }
        };
        var eventViewModels = new List<LifeEventViewModel>
        {
            new LifeEventViewModel { Id = 1, PersonId = 1, EventType = "Birth", Title = "Parent Birth" },
            new LifeEventViewModel { Id = 2, PersonId = 2, EventType = "Birth", Title = "Child Birth" }
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _lifeEventRepository.GetByPersonIdAsync(1)).Returns(parentEvents);
        A.CallTo(() => _lifeEventRepository.GetByPersonIdAsync(2)).Returns(childEvents);
        A.CallTo(() => _lifeEventMapper.MapToViewModels(A<List<LifeEvent>>._)).Returns(eventViewModels);

        // Act
        var result = await _service.GetRelatedEventsAsync(relationshipId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, e => e.Title == "Parent Birth");
        Assert.Contains(result, e => e.Title == "Child Birth");
    }

    [Fact]
    public async Task GetRelatedEventsAsync_ThrowsNotFoundException_WhenRelationshipNotFound()
    {
        // Arrange
        var relationshipId = 999;
        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns((ParentChild?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetRelatedEventsAsync(relationshipId));
    }

    [Fact]
    public async Task GetGrandparentsAsync_ReturnsPersonViewModels()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var grandparents = new List<Person>
        {
            new Person { Id = 10, FirstName = "GrandPa", LastName = "Smith" },
            new Person { Id = 11, FirstName = "GrandMa", LastName = "Smith" }
        };
        var grandparentViewModels = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 10, FirstName = "GrandPa", LastName = "Smith" },
            new PersonViewModel { Id = 11, FirstName = "GrandMa", LastName = "Smith" }
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _parentChildRepository.GetGrandparentsAsync(relationshipId)).Returns(grandparents);
        A.CallTo(() => _personMapper.MapToViewModel(grandparents[0])).Returns(grandparentViewModels[0]);
        A.CallTo(() => _personMapper.MapToViewModel(grandparents[1])).Returns(grandparentViewModels[1]);

        // Act
        var result = await _service.GetGrandparentsAsync(relationshipId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.FirstName == "GrandPa");
        Assert.Contains(result, p => p.FirstName == "GrandMa");
    }

    [Fact]
    public async Task GetGrandparentsAsync_ThrowsNotFoundException_WhenRelationshipNotFound()
    {
        // Arrange
        var relationshipId = 999;
        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns((ParentChild?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetGrandparentsAsync(relationshipId));
    }

    [Fact]
    public async Task GetSiblingsAsync_ReturnsPersonViewModels()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var siblings = new List<Person>
        {
            new Person { Id = 3, FirstName = "Jack", LastName = "Doe" },
            new Person { Id = 4, FirstName = "Jill", LastName = "Doe" }
        };
        var siblingViewModels = new List<PersonViewModel>
        {
            new PersonViewModel { Id = 3, FirstName = "Jack", LastName = "Doe" },
            new PersonViewModel { Id = 4, FirstName = "Jill", LastName = "Doe" }
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _parentChildRepository.GetSiblingsAsync(relationshipId)).Returns(siblings);
        A.CallTo(() => _personMapper.MapToViewModel(siblings[0])).Returns(siblingViewModels[0]);
        A.CallTo(() => _personMapper.MapToViewModel(siblings[1])).Returns(siblingViewModels[1]);

        // Act
        var result = await _service.GetSiblingsAsync(relationshipId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.FirstName == "Jack");
        Assert.Contains(result, p => p.FirstName == "Jill");
    }

    [Fact]
    public async Task GetSiblingsAsync_ThrowsNotFoundException_WhenRelationshipNotFound()
    {
        // Arrange
        var relationshipId = 999;
        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns((ParentChild?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetSiblingsAsync(relationshipId));
    }

    [Fact]
    public async Task GetEvidenceAsync_ReturnsEmptyList_WhenNoSourcesLinked()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var sources = new List<Source>();
        var sourceViewModels = new List<SourceViewModel>();

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _parentChildRepository.GetSourcesAsync(relationshipId)).Returns(sources);
        A.CallTo(() => _sourceMapper.MapToViewModels(sources)).Returns(sourceViewModels);

        // Act
        var result = await _service.GetEvidenceAsync(relationshipId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGrandparentsAsync_ReturnsEmptyList_WhenNoGrandparents()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var grandparents = new List<Person>();

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _parentChildRepository.GetGrandparentsAsync(relationshipId)).Returns(grandparents);

        // Act
        var result = await _service.GetGrandparentsAsync(relationshipId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSiblingsAsync_ReturnsEmptyList_WhenNoSiblings()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild { Id = relationshipId, ParentPersonId = 1, ChildPersonId = 2 };
        var siblings = new List<Person>();

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);
        A.CallTo(() => _parentChildRepository.GetSiblingsAsync(relationshipId)).Returns(siblings);

        // Act
        var result = await _service.GetSiblingsAsync(relationshipId);

        // Assert
        Assert.Empty(result);
    }
}
