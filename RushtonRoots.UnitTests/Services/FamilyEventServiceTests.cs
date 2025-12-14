using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class FamilyEventServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsFamilyEventViewModel_WhenEventExists()
    {
        // Arrange
        var eventId = 1;
        var familyEvent = new FamilyEvent
        {
            Id = eventId,
            Title = "Family Reunion 2025",
            Description = "Annual family gathering",
            StartDateTime = DateTime.UtcNow.AddMonths(1),
            EventType = "Reunion",
            CreatedByUserId = "user1",
            CreatedByUser = new ApplicationUser { Id = "user1", UserName = "John Doe" }
        };

        var expectedViewModel = new FamilyEventViewModel
        {
            Id = eventId,
            Title = "Family Reunion 2025",
            Description = "Annual family gathering",
            EventType = "Reunion",
            CreatedByUserId = "user1",
            CreatedByUserName = "John Doe"
        };

        var mockRepository = A.Fake<IFamilyEventRepository>();
        var mockMapper = A.Fake<IFamilyEventMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(eventId)).Returns(familyEvent);
        A.CallTo(() => mockMapper.MapToViewModel(familyEvent)).Returns(expectedViewModel);

        var service = new FamilyEventService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetByIdAsync(eventId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eventId, result.Id);
        Assert.Equal("Family Reunion 2025", result.Title);
        Assert.Equal("Reunion", result.EventType);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEventDoesNotExist()
    {
        // Arrange
        var mockRepository = A.Fake<IFamilyEventRepository>();
        var mockMapper = A.Fake<IFamilyEventMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(999)).Returns((FamilyEvent?)null);

        var service = new FamilyEventService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEventAsync_CreatesEventAndNotification()
    {
        // Arrange
        var userId = "user1";
        var request = new CreateFamilyEventRequest
        {
            Title = "Birthday Party",
            Description = "John's 50th birthday",
            StartDateTime = DateTime.UtcNow.AddDays(30),
            EventType = "Birthday"
        };

        var familyEvent = new FamilyEvent
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
            StartDateTime = request.StartDateTime,
            EventType = request.EventType,
            CreatedByUserId = userId
        };

        var expectedViewModel = new FamilyEventViewModel
        {
            Id = 1,
            Title = "Birthday Party",
            EventType = "Birthday"
        };

        var mockRepository = A.Fake<IFamilyEventRepository>();
        var mockMapper = A.Fake<IFamilyEventMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockMapper.MapToEntity(request, userId)).Returns(familyEvent);
        A.CallTo(() => mockRepository.AddAsync(familyEvent)).Returns(familyEvent);
        A.CallTo(() => mockMapper.MapToViewModel(familyEvent)).Returns(expectedViewModel);

        var service = new FamilyEventService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.CreateEventAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Birthday Party", result.Title);
        A.CallTo(() => mockNotificationService.CreateNotificationAsync(
            userId,
            "Event",
            A<string>._,
            A<string>._,
            A<string>._,
            A<int>._,
            A<string>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateEventAsync_ThrowsUnauthorizedException_WhenUserIsNotCreator()
    {
        // Arrange
        var eventId = 1;
        var creatorUserId = "user1";
        var otherUserId = "user2";

        var familyEvent = new FamilyEvent
        {
            Id = eventId,
            Title = "Event",
            CreatedByUserId = creatorUserId
        };

        var request = new UpdateFamilyEventRequest
        {
            Id = eventId,
            Title = "Updated Event"
        };

        var mockRepository = A.Fake<IFamilyEventRepository>();
        var mockMapper = A.Fake<IFamilyEventMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(eventId)).Returns(familyEvent);

        var service = new FamilyEventService(mockRepository, mockMapper, mockNotificationService);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.UpdateEventAsync(eventId, request, otherUserId));
    }

    [Fact]
    public async Task GetUpcomingEventsAsync_ReturnsUpcomingEvents()
    {
        // Arrange
        var events = new List<FamilyEvent>
        {
            new FamilyEvent { Id = 1, Title = "Event 1", StartDateTime = DateTime.UtcNow.AddDays(1) },
            new FamilyEvent { Id = 2, Title = "Event 2", StartDateTime = DateTime.UtcNow.AddDays(7) }
        };

        var viewModels = new List<FamilyEventViewModel>
        {
            new FamilyEventViewModel { Id = 1, Title = "Event 1" },
            new FamilyEventViewModel { Id = 2, Title = "Event 2" }
        };

        var mockRepository = A.Fake<IFamilyEventRepository>();
        var mockMapper = A.Fake<IFamilyEventMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetUpcomingEventsAsync(10)).Returns(events);
        A.CallTo(() => mockMapper.MapToViewModel(events[0])).Returns(viewModels[0]);
        A.CallTo(() => mockMapper.MapToViewModel(events[1])).Returns(viewModels[1]);

        var service = new FamilyEventService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetUpcomingEventsAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
