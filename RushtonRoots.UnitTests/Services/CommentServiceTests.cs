using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class CommentServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsCommentViewModel_WhenCommentExists()
    {
        // Arrange
        var commentId = 1;
        var comment = new Comment
        {
            Id = commentId,
            Content = "Great photo!",
            UserId = "user1",
            EntityType = "Media",
            EntityId = 5,
            User = new ApplicationUser { Id = "user1", UserName = "John Doe" }
        };

        var expectedViewModel = new CommentViewModel
        {
            Id = commentId,
            Content = "Great photo!",
            UserId = "user1",
            UserName = "John Doe",
            EntityType = "Media",
            EntityId = 5
        };

        var mockRepository = A.Fake<ICommentRepository>();
        var mockMapper = A.Fake<ICommentMapper>();

        A.CallTo(() => mockRepository.GetByIdAsync(commentId)).Returns(comment);
        A.CallTo(() => mockMapper.MapToViewModel(comment)).Returns(expectedViewModel);

        var service = new CommentService(mockRepository, mockMapper);

        // Act
        var result = await service.GetByIdAsync(commentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal("Great photo!", result.Content);
        Assert.Equal("Media", result.EntityType);
    }

    [Fact]
    public async Task CreateCommentAsync_CreatesComment()
    {
        // Arrange
        var userId = "user1";
        var request = new CreateCommentRequest
        {
            Content = "This is amazing!",
            EntityType = "Person",
            EntityId = 10
        };

        var comment = new Comment
        {
            Id = 1,
            Content = request.Content,
            UserId = userId,
            EntityType = request.EntityType,
            EntityId = request.EntityId
        };

        var expectedViewModel = new CommentViewModel
        {
            Id = 1,
            Content = "This is amazing!",
            EntityType = "Person",
            EntityId = 10
        };

        var mockRepository = A.Fake<ICommentRepository>();
        var mockMapper = A.Fake<ICommentMapper>();

        A.CallTo(() => mockMapper.MapToEntity(request, userId)).Returns(comment);
        A.CallTo(() => mockRepository.AddAsync(comment)).Returns(comment);
        A.CallTo(() => mockMapper.MapToViewModel(comment)).Returns(expectedViewModel);

        var service = new CommentService(mockRepository, mockMapper);

        // Act
        var result = await service.CreateCommentAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("This is amazing!", result.Content);
        Assert.Equal("Person", result.EntityType);
    }

    [Fact]
    public async Task UpdateCommentAsync_UpdatesCommentAndMarksAsEdited()
    {
        // Arrange
        var commentId = 1;
        var userId = "user1";
        var comment = new Comment
        {
            Id = commentId,
            Content = "Original content",
            UserId = userId,
            IsEdited = false
        };

        var request = new UpdateCommentRequest
        {
            Id = commentId,
            Content = "Updated content"
        };

        var expectedViewModel = new CommentViewModel
        {
            Id = commentId,
            Content = "Updated content",
            IsEdited = true
        };

        var mockRepository = A.Fake<ICommentRepository>();
        var mockMapper = A.Fake<ICommentMapper>();

        A.CallTo(() => mockRepository.GetByIdAsync(commentId)).Returns(comment);
        A.CallTo(() => mockRepository.UpdateAsync(comment)).Returns(comment);
        A.CallTo(() => mockMapper.MapToViewModel(comment)).Returns(expectedViewModel);

        var service = new CommentService(mockRepository, mockMapper);

        // Act
        var result = await service.UpdateCommentAsync(commentId, request, userId);

        // Assert
        Assert.NotNull(result);
        A.CallTo(() => mockMapper.UpdateEntity(comment, request)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateCommentAsync_ThrowsUnauthorizedException_WhenUserIsNotOwner()
    {
        // Arrange
        var commentId = 1;
        var ownerId = "user1";
        var otherUserId = "user2";
        var comment = new Comment
        {
            Id = commentId,
            Content = "Original content",
            UserId = ownerId
        };

        var request = new UpdateCommentRequest
        {
            Id = commentId,
            Content = "Updated content"
        };

        var mockRepository = A.Fake<ICommentRepository>();
        var mockMapper = A.Fake<ICommentMapper>();

        A.CallTo(() => mockRepository.GetByIdAsync(commentId)).Returns(comment);

        var service = new CommentService(mockRepository, mockMapper);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.UpdateCommentAsync(commentId, request, otherUserId));
    }

    [Fact]
    public async Task GetByEntityAsync_ReturnsCommentsForEntity()
    {
        // Arrange
        var comments = new List<Comment>
        {
            new Comment { Id = 1, Content = "Comment 1", EntityType = "Media", EntityId = 5 },
            new Comment { Id = 2, Content = "Comment 2", EntityType = "Media", EntityId = 5 }
        };

        var viewModels = new List<CommentViewModel>
        {
            new CommentViewModel { Id = 1, Content = "Comment 1" },
            new CommentViewModel { Id = 2, Content = "Comment 2" }
        };

        var mockRepository = A.Fake<ICommentRepository>();
        var mockMapper = A.Fake<ICommentMapper>();

        A.CallTo(() => mockRepository.GetByEntityAsync("Media", 5)).Returns(comments);
        A.CallTo(() => mockMapper.MapToViewModel(comments[0])).Returns(viewModels[0]);
        A.CallTo(() => mockMapper.MapToViewModel(comments[1])).Returns(viewModels[1]);

        var service = new CommentService(mockRepository, mockMapper);

        // Act
        var result = await service.GetByEntityAsync("Media", 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
