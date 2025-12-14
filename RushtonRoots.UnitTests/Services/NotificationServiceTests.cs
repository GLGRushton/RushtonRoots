using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class NotificationServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsNotificationViewModel_WhenNotificationExists()
    {
        // Arrange
        var notificationId = 1;
        var notification = new Notification
        {
            Id = notificationId,
            UserId = "user1",
            Type = "Message",
            Title = "New Message",
            Message = "You have a new message",
            IsRead = false
        };

        var expectedViewModel = new NotificationViewModel
        {
            Id = notificationId,
            UserId = "user1",
            Type = "Message",
            Title = "New Message",
            Message = "You have a new message",
            IsRead = false
        };

        var mockRepository = A.Fake<INotificationRepository>();
        var mockMapper = A.Fake<INotificationMapper>();
        var mockEmailService = A.Fake<IEmailService>();

        A.CallTo(() => mockRepository.GetByIdAsync(notificationId)).Returns(notification);
        A.CallTo(() => mockMapper.MapToViewModel(notification)).Returns(expectedViewModel);

        var service = new NotificationService(mockRepository, mockMapper, mockEmailService);

        // Act
        var result = await service.GetByIdAsync(notificationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Id, result.Id);
        Assert.Equal(expectedViewModel.Type, result.Type);
        Assert.Equal(expectedViewModel.Title, result.Title);
    }

    [Fact]
    public async Task CreateNotificationAsync_CreatesNotification()
    {
        // Arrange
        var userId = "user1";
        var type = "Message";
        var title = "New Message";
        var message = "You have a new message";
        var actionUrl = "/messages/1";
        var relatedEntityId = 1;
        var relatedEntityType = "Message";

        var notification = new Notification
        {
            Id = 1,
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            User = new ApplicationUser { Id = userId, Email = "test@example.com" }
        };

        var preference = new NotificationPreference
        {
            UserId = userId,
            NotificationType = type,
            InAppEnabled = true,
            EmailEnabled = false,
            EmailFrequency = "Immediate"
        };

        var preferenceViewModel = new NotificationPreferenceViewModel
        {
            UserId = userId,
            NotificationType = type,
            InAppEnabled = true,
            EmailEnabled = false,
            EmailFrequency = "Immediate"
        };

        var expectedViewModel = new NotificationViewModel
        {
            Id = 1,
            UserId = userId,
            Type = type,
            Title = title,
            Message = message
        };

        var mockRepository = A.Fake<INotificationRepository>();
        var mockMapper = A.Fake<INotificationMapper>();
        var mockEmailService = A.Fake<IEmailService>();

        A.CallTo(() => mockRepository.AddAsync(A<Notification>._)).Returns(notification);
        A.CallTo(() => mockMapper.MapToViewModel(notification)).Returns(expectedViewModel);
        A.CallTo(() => mockRepository.GetPreferenceAsync(userId, type)).Returns(preference);
        A.CallTo(() => mockMapper.MapToViewModel(preference)).Returns(preferenceViewModel);

        var service = new NotificationService(mockRepository, mockMapper, mockEmailService);

        // Act
        var result = await service.CreateNotificationAsync(userId, type, title, message, actionUrl, relatedEntityId, relatedEntityType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Title, result.Title);
        A.CallTo(() => mockRepository.AddAsync(A<Notification>._)).MustHaveHappened();
    }

    [Fact]
    public async Task MarkAsReadAsync_UpdatesNotification()
    {
        // Arrange
        var notificationId = 1;
        var userId = "user1";
        var notification = new Notification
        {
            Id = notificationId,
            UserId = userId,
            Type = "Message",
            Title = "Test",
            Message = "Test message",
            IsRead = false
        };

        var mockRepository = A.Fake<INotificationRepository>();
        var mockMapper = A.Fake<INotificationMapper>();
        var mockEmailService = A.Fake<IEmailService>();

        A.CallTo(() => mockRepository.GetByIdAsync(notificationId)).Returns(notification);
        A.CallTo(() => mockRepository.UpdateAsync(notification)).Returns(notification);

        var service = new NotificationService(mockRepository, mockMapper, mockEmailService);

        // Act
        await service.MarkAsReadAsync(notificationId, userId);

        // Assert
        Assert.True(notification.IsRead);
        Assert.NotNull(notification.ReadAt);
        A.CallTo(() => mockRepository.UpdateAsync(notification)).MustHaveHappened();
    }

    [Fact]
    public async Task UpdatePreferenceAsync_CreatesNewPreference_WhenNotExists()
    {
        // Arrange
        var userId = "user1";
        var request = new UpdateNotificationPreferenceRequest
        {
            NotificationType = "Message",
            InAppEnabled = true,
            EmailEnabled = false,
            EmailFrequency = "Daily"
        };

        var newPreference = new NotificationPreference
        {
            Id = 1,
            UserId = userId,
            NotificationType = request.NotificationType,
            InAppEnabled = request.InAppEnabled,
            EmailEnabled = request.EmailEnabled,
            EmailFrequency = request.EmailFrequency
        };

        var expectedViewModel = new NotificationPreferenceViewModel
        {
            Id = 1,
            UserId = userId,
            NotificationType = request.NotificationType,
            InAppEnabled = request.InAppEnabled,
            EmailEnabled = request.EmailEnabled,
            EmailFrequency = request.EmailFrequency
        };

        var mockRepository = A.Fake<INotificationRepository>();
        var mockMapper = A.Fake<INotificationMapper>();
        var mockEmailService = A.Fake<IEmailService>();

        A.CallTo(() => mockRepository.GetPreferenceAsync(userId, request.NotificationType)).Returns<NotificationPreference?>(null);
        A.CallTo(() => mockMapper.MapToEntity(request, userId)).Returns(newPreference);
        A.CallTo(() => mockRepository.AddPreferenceAsync(newPreference)).Returns(newPreference);
        A.CallTo(() => mockMapper.MapToViewModel(newPreference)).Returns(expectedViewModel);

        var service = new NotificationService(mockRepository, mockMapper, mockEmailService);

        // Act
        var result = await service.UpdatePreferenceAsync(userId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.NotificationType, result.NotificationType);
        A.CallTo(() => mockRepository.AddPreferenceAsync(A<NotificationPreference>._)).MustHaveHappened();
    }
}
