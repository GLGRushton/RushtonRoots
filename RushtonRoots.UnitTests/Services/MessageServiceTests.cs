using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class MessageServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsMessageViewModel_WhenMessageExists()
    {
        // Arrange
        var messageId = 1;
        var message = new Message
        {
            Id = messageId,
            Content = "Test message",
            SenderUserId = "user1",
            RecipientUserId = "user2",
            Sender = new ApplicationUser { Id = "user1", UserName = "John Doe" },
            Recipient = new ApplicationUser { Id = "user2", UserName = "Jane Doe" }
        };

        var expectedViewModel = new MessageViewModel
        {
            Id = messageId,
            Content = "Test message",
            SenderUserId = "user1",
            SenderName = "John Doe",
            RecipientUserId = "user2",
            RecipientName = "Jane Doe"
        };

        var mockRepository = A.Fake<IMessageRepository>();
        var mockMapper = A.Fake<IMessageMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(messageId)).Returns(message);
        A.CallTo(() => mockMapper.MapToViewModel(message)).Returns(expectedViewModel);

        var service = new MessageService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetByIdAsync(messageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Id, result.Id);
        Assert.Equal(expectedViewModel.Content, result.Content);
        Assert.Equal(expectedViewModel.SenderUserId, result.SenderUserId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMessageDoesNotExist()
    {
        // Arrange
        var messageId = 999;
        var mockRepository = A.Fake<IMessageRepository>();
        var mockMapper = A.Fake<IMessageMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(messageId)).Returns((Message?)null);

        var service = new MessageService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetByIdAsync(messageId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SendMessageAsync_CreatesMessageAndNotification()
    {
        // Arrange
        var request = new CreateMessageRequest
        {
            Content = "Hello!",
            RecipientUserId = "user2",
            MentionedUserIds = new List<string>()
        };

        var senderUserId = "user1";
        var message = new Message
        {
            Id = 1,
            Content = request.Content,
            SenderUserId = senderUserId,
            RecipientUserId = request.RecipientUserId,
            Sender = new ApplicationUser { Id = senderUserId, UserName = "John" }
        };

        var expectedViewModel = new MessageViewModel
        {
            Id = 1,
            Content = request.Content,
            SenderUserId = senderUserId
        };

        var mockRepository = A.Fake<IMessageRepository>();
        var mockMapper = A.Fake<IMessageMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockMapper.MapToEntity(request, senderUserId)).Returns(message);
        A.CallTo(() => mockRepository.AddAsync(message)).Returns(message);
        A.CallTo(() => mockMapper.MapToViewModel(message)).Returns(expectedViewModel);

        var service = new MessageService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.SendMessageAsync(request, senderUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedViewModel.Content, result.Content);
        A.CallTo(() => mockNotificationService.CreateNotificationAsync(
            A<string>._, A<string>._, A<string>._, A<string>._, A<string?>._, A<int?>._, A<string?>._))
            .MustHaveHappened();
    }

    [Fact]
    public async Task MarkAsReadAsync_UpdatesMessage()
    {
        // Arrange
        var messageId = 1;
        var userId = "user2";
        var message = new Message
        {
            Id = messageId,
            Content = "Test",
            SenderUserId = "user1",
            RecipientUserId = userId,
            ReadAt = null
        };

        var mockRepository = A.Fake<IMessageRepository>();
        var mockMapper = A.Fake<IMessageMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(messageId)).Returns(message);
        A.CallTo(() => mockRepository.UpdateAsync(message)).Returns(message);

        var service = new MessageService(mockRepository, mockMapper, mockNotificationService);

        // Act
        await service.MarkAsReadAsync(messageId, userId);

        // Assert
        Assert.NotNull(message.ReadAt);
        A.CallTo(() => mockRepository.UpdateAsync(message)).MustHaveHappened();
    }

    [Fact]
    public async Task UpdateMessageAsync_ThrowsUnauthorized_WhenUserIsNotSender()
    {
        // Arrange
        var messageId = 1;
        var userId = "user2";
        var message = new Message
        {
            Id = messageId,
            Content = "Original",
            SenderUserId = "user1"
        };

        var request = new UpdateMessageRequest { Content = "Updated" };

        var mockRepository = A.Fake<IMessageRepository>();
        var mockMapper = A.Fake<IMessageMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(messageId)).Returns(message);

        var service = new MessageService(mockRepository, mockMapper, mockNotificationService);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpdateMessageAsync(messageId, request, userId));
    }
}
