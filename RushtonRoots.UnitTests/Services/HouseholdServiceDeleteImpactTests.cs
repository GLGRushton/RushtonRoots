using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Application.Validators;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

/// <summary>
/// Tests for household delete impact calculation service methods.
/// </summary>
public class HouseholdServiceDeleteImpactTests
{
    private readonly IHouseholdRepository _mockRepository;
    private readonly IHouseholdMapper _mockMapper;
    private readonly IHouseholdValidator _mockValidator;
    private readonly HouseholdService _service;

    public HouseholdServiceDeleteImpactTests()
    {
        _mockRepository = A.Fake<IHouseholdRepository>();
        _mockMapper = A.Fake<IHouseholdMapper>();
        _mockValidator = A.Fake<IHouseholdValidator>();
        _service = new HouseholdService(_mockRepository, _mockMapper, _mockValidator);
    }

    #region GetDeleteImpactAsync Tests

    [Fact]
    public async Task GetDeleteImpactAsync_WithValidHouseholdId_ReturnsImpactData()
    {
        // Arrange
        var householdId = 1;
        var expectedMemberCount = 5;
        var expectedPhotoCount = 15;
        var expectedDocumentCount = 8;
        var expectedRelationshipCount = 12;
        var expectedEventCount = 3;

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetMemberCountAsync(householdId)).Returns(expectedMemberCount);
        A.CallTo(() => _mockRepository.GetPhotoCountAsync(householdId)).Returns(expectedPhotoCount);
        A.CallTo(() => _mockRepository.GetDocumentCountAsync(householdId)).Returns(expectedDocumentCount);
        A.CallTo(() => _mockRepository.GetRelationshipCountAsync(householdId)).Returns(expectedRelationshipCount);
        A.CallTo(() => _mockRepository.GetEventCountAsync(householdId)).Returns(expectedEventCount);

        // Act
        var result = await _service.GetDeleteImpactAsync(householdId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMemberCount, result.MemberCount);
        Assert.Equal(expectedPhotoCount, result.PhotoCount);
        Assert.Equal(expectedDocumentCount, result.DocumentCount);
        Assert.Equal(expectedRelationshipCount, result.RelationshipCount);
        Assert.Equal(expectedEventCount, result.EventCount);

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetMemberCountAsync(householdId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetPhotoCountAsync(householdId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetDocumentCountAsync(householdId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetRelationshipCountAsync(householdId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _mockRepository.GetEventCountAsync(householdId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetDeleteImpactAsync_WithZeroCounts_ReturnsZeroImpact()
    {
        // Arrange
        var householdId = 1;

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetMemberCountAsync(householdId)).Returns(0);
        A.CallTo(() => _mockRepository.GetPhotoCountAsync(householdId)).Returns(0);
        A.CallTo(() => _mockRepository.GetDocumentCountAsync(householdId)).Returns(0);
        A.CallTo(() => _mockRepository.GetRelationshipCountAsync(householdId)).Returns(0);
        A.CallTo(() => _mockRepository.GetEventCountAsync(householdId)).Returns(0);

        // Act
        var result = await _service.GetDeleteImpactAsync(householdId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.MemberCount);
        Assert.Equal(0, result.PhotoCount);
        Assert.Equal(0, result.DocumentCount);
        Assert.Equal(0, result.RelationshipCount);
        Assert.Equal(0, result.EventCount);
    }

    [Fact]
    public async Task GetDeleteImpactAsync_WithInvalidHouseholdId_ThrowsValidationException()
    {
        // Arrange
        var householdId = 0;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _service.GetDeleteImpactAsync(householdId));
    }

    [Fact]
    public async Task GetDeleteImpactAsync_WithNegativeHouseholdId_ThrowsValidationException()
    {
        // Arrange
        var householdId = -5;

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _service.GetDeleteImpactAsync(householdId));
    }

    [Fact]
    public async Task GetDeleteImpactAsync_WithNonExistentHousehold_ThrowsNotFoundException()
    {
        // Arrange
        var householdId = 999;

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.GetDeleteImpactAsync(householdId));

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetDeleteImpactAsync_WithLargeNumbers_ReturnsCorrectCounts()
    {
        // Arrange
        var householdId = 1;
        var expectedMemberCount = 100;
        var expectedPhotoCount = 5000;
        var expectedDocumentCount = 1000;
        var expectedRelationshipCount = 250;
        var expectedEventCount = 50;

        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetMemberCountAsync(householdId)).Returns(expectedMemberCount);
        A.CallTo(() => _mockRepository.GetPhotoCountAsync(householdId)).Returns(expectedPhotoCount);
        A.CallTo(() => _mockRepository.GetDocumentCountAsync(householdId)).Returns(expectedDocumentCount);
        A.CallTo(() => _mockRepository.GetRelationshipCountAsync(householdId)).Returns(expectedRelationshipCount);
        A.CallTo(() => _mockRepository.GetEventCountAsync(householdId)).Returns(expectedEventCount);

        // Act
        var result = await _service.GetDeleteImpactAsync(householdId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMemberCount, result.MemberCount);
        Assert.Equal(expectedPhotoCount, result.PhotoCount);
        Assert.Equal(expectedDocumentCount, result.DocumentCount);
        Assert.Equal(expectedRelationshipCount, result.RelationshipCount);
        Assert.Equal(expectedEventCount, result.EventCount);
    }

    [Fact]
    public async Task GetDeleteImpactAsync_WithSomeZeroSomeNonZero_ReturnsCorrectMix()
    {
        // Arrange
        var householdId = 1;
        
        A.CallTo(() => _mockRepository.ExistsAsync(householdId)).Returns(true);
        A.CallTo(() => _mockRepository.GetMemberCountAsync(householdId)).Returns(3);
        A.CallTo(() => _mockRepository.GetPhotoCountAsync(householdId)).Returns(0);
        A.CallTo(() => _mockRepository.GetDocumentCountAsync(householdId)).Returns(5);
        A.CallTo(() => _mockRepository.GetRelationshipCountAsync(householdId)).Returns(0);
        A.CallTo(() => _mockRepository.GetEventCountAsync(householdId)).Returns(2);

        // Act
        var result = await _service.GetDeleteImpactAsync(householdId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.MemberCount);
        Assert.Equal(0, result.PhotoCount);
        Assert.Equal(5, result.DocumentCount);
        Assert.Equal(0, result.RelationshipCount);
        Assert.Equal(2, result.EventCount);
    }

    #endregion
}
