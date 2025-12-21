using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

/// <summary>
/// Tests for ParentChild service verification methods (Phase 4.3).
/// </summary>
public class ParentChildServiceVerificationTests
{
    private readonly IParentChildRepository _parentChildRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ILifeEventRepository _lifeEventRepository;
    private readonly IParentChildMapper _parentChildMapper;
    private readonly ISourceMapper _sourceMapper;
    private readonly ILifeEventMapper _lifeEventMapper;
    private readonly IPersonMapper _personMapper;
    private readonly ParentChildService _service;

    public ParentChildServiceVerificationTests()
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

    #region VerifyRelationshipAsync Tests

    [Fact]
    public async Task VerifyRelationshipAsync_SetsVerificationFields()
    {
        // Arrange
        var relationshipId = 1;
        var verifiedBy = "admin@test.com";
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            IsVerified = false,
            VerifiedDate = null,
            VerifiedBy = null
        };

        var verifiedRelationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            IsVerified = true,
            VerifiedDate = DateTime.UtcNow,
            VerifiedBy = verifiedBy
        };

        var viewModel = new ParentChildViewModel
        {
            Id = relationshipId,
            ParentName = "Parent",
            ChildName = "Child"
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId))
            .ReturnsNextFromSequence(relationship, verifiedRelationship);
        A.CallTo(() => _parentChildRepository.UpdateAsync(A<ParentChild>._)).Returns(verifiedRelationship);
        A.CallTo(() => _parentChildMapper.MapToViewModel(verifiedRelationship)).Returns(viewModel);

        // Act
        var result = await _service.VerifyRelationshipAsync(relationshipId, verifiedBy);

        // Assert
        Assert.NotNull(result);
        A.CallTo(() => _parentChildRepository.UpdateAsync(A<ParentChild>.That.Matches(pc =>
            pc.IsVerified &&
            pc.VerifiedBy == verifiedBy &&
            pc.VerifiedDate != null)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task VerifyRelationshipAsync_ThrowsNotFoundException_WhenRelationshipNotFound()
    {
        // Arrange
        var relationshipId = 999;
        var verifiedBy = "admin@test.com";
        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns((ParentChild?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.VerifyRelationshipAsync(relationshipId, verifiedBy));
    }

    [Fact]
    public async Task VerifyRelationshipAsync_ThrowsValidationException_WhenVerifiedByIsNull()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.VerifyRelationshipAsync(relationshipId, null!));
    }

    [Fact]
    public async Task VerifyRelationshipAsync_ThrowsValidationException_WhenVerifiedByIsEmpty()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.VerifyRelationshipAsync(relationshipId, string.Empty));
    }

    [Fact]
    public async Task VerifyRelationshipAsync_ThrowsValidationException_WhenVerifiedByIsWhitespace()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.VerifyRelationshipAsync(relationshipId, "   "));
    }

    #endregion

    #region UpdateNotesAsync Tests

    [Fact]
    public async Task UpdateNotesAsync_UpdatesNotesField()
    {
        // Arrange
        var relationshipId = 1;
        var notes = "This relationship is verified through birth certificate dated 1990-05-15.";
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = null
        };

        var updatedRelationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = notes
        };

        var viewModel = new ParentChildViewModel
        {
            Id = relationshipId,
            ParentName = "Parent",
            ChildName = "Child",
            Notes = notes
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId))
            .ReturnsNextFromSequence(relationship, updatedRelationship);
        A.CallTo(() => _parentChildRepository.UpdateAsync(A<ParentChild>._)).Returns(A.Dummy<ParentChild>());
        A.CallTo(() => _parentChildMapper.MapToViewModel(updatedRelationship)).Returns(viewModel);

        // Act
        var result = await _service.UpdateNotesAsync(relationshipId, notes);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notes, result.Notes);
        A.CallTo(() => _parentChildRepository.UpdateAsync(A<ParentChild>.That.Matches(pc =>
            pc.Notes == notes)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateNotesAsync_ThrowsNotFoundException_WhenRelationshipNotFound()
    {
        // Arrange
        var relationshipId = 999;
        var notes = "Test notes";
        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns((ParentChild?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateNotesAsync(relationshipId, notes));
    }

    [Fact]
    public async Task UpdateNotesAsync_ThrowsValidationException_WhenNotesIsNull()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.UpdateNotesAsync(relationshipId, null!));
    }

    [Fact]
    public async Task UpdateNotesAsync_ThrowsValidationException_WhenNotesExceedsMaxLength()
    {
        // Arrange
        var relationshipId = 1;
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        var longNotes = new string('A', 2001); // 2001 characters, exceeds 2000 limit

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId)).Returns(relationship);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.UpdateNotesAsync(relationshipId, longNotes));
    }

    [Fact]
    public async Task UpdateNotesAsync_AcceptsEmptyString()
    {
        // Arrange
        var relationshipId = 1;
        var notes = string.Empty;
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = "Old notes"
        };

        var updatedRelationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = notes
        };

        var viewModel = new ParentChildViewModel
        {
            Id = relationshipId,
            ParentName = "Parent",
            ChildName = "Child",
            Notes = notes
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId))
            .ReturnsNextFromSequence(relationship, updatedRelationship);
        A.CallTo(() => _parentChildRepository.UpdateAsync(A<ParentChild>._)).Returns(A.Dummy<ParentChild>());
        A.CallTo(() => _parentChildMapper.MapToViewModel(updatedRelationship)).Returns(viewModel);

        // Act
        var result = await _service.UpdateNotesAsync(relationshipId, notes);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notes, result.Notes);
    }

    [Fact]
    public async Task UpdateNotesAsync_AcceptsMaxLengthNotes()
    {
        // Arrange
        var relationshipId = 1;
        var notes = new string('A', 2000); // Exactly 2000 characters
        var relationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological"
        };

        var updatedRelationship = new ParentChild
        {
            Id = relationshipId,
            ParentPersonId = 1,
            ChildPersonId = 2,
            RelationshipType = "Biological",
            Notes = notes
        };

        var viewModel = new ParentChildViewModel
        {
            Id = relationshipId,
            ParentName = "Parent",
            ChildName = "Child",
            Notes = notes
        };

        A.CallTo(() => _parentChildRepository.GetByIdAsync(relationshipId))
            .ReturnsNextFromSequence(relationship, updatedRelationship);
        A.CallTo(() => _parentChildRepository.UpdateAsync(A<ParentChild>._)).Returns(A.Dummy<ParentChild>());
        A.CallTo(() => _parentChildMapper.MapToViewModel(updatedRelationship)).Returns(viewModel);

        // Act
        var result = await _service.UpdateNotesAsync(relationshipId, notes);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notes, result.Notes);
    }

    #endregion
}
