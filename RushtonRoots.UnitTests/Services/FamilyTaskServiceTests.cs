using FakeItEasy;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Services;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using Xunit;

namespace RushtonRoots.UnitTests.Services;

public class FamilyTaskServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsFamilyTaskViewModel_WhenTaskExists()
    {
        // Arrange
        var taskId = 1;
        var familyTask = new FamilyTask
        {
            Id = taskId,
            Title = "Buy decorations",
            Description = "Get decorations for the reunion",
            Status = "Pending",
            Priority = "High",
            CreatedByUserId = "user1",
            CreatedByUser = new ApplicationUser { Id = "user1", UserName = "John Doe" }
        };

        var expectedViewModel = new FamilyTaskViewModel
        {
            Id = taskId,
            Title = "Buy decorations",
            Status = "Pending",
            Priority = "High"
        };

        var mockRepository = A.Fake<IFamilyTaskRepository>();
        var mockMapper = A.Fake<IFamilyTaskMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(taskId)).Returns(familyTask);
        A.CallTo(() => mockMapper.MapToViewModel(familyTask)).Returns(expectedViewModel);

        var service = new FamilyTaskService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetByIdAsync(taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal("Buy decorations", result.Title);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task CreateTaskAsync_CreatesTaskAndNotifiesAssignedUser()
    {
        // Arrange
        var creatorUserId = "user1";
        var assignedUserId = "user2";
        var request = new CreateFamilyTaskRequest
        {
            Title = "Book venue",
            Description = "Reserve the community center",
            Status = "Pending",
            Priority = "High",
            AssignedToUserId = assignedUserId
        };

        var familyTask = new FamilyTask
        {
            Id = 1,
            Title = request.Title,
            Description = request.Description,
            CreatedByUserId = creatorUserId,
            AssignedToUserId = assignedUserId
        };

        var expectedViewModel = new FamilyTaskViewModel
        {
            Id = 1,
            Title = "Book venue"
        };

        var mockRepository = A.Fake<IFamilyTaskRepository>();
        var mockMapper = A.Fake<IFamilyTaskMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockMapper.MapToEntity(request, creatorUserId)).Returns(familyTask);
        A.CallTo(() => mockRepository.AddAsync(familyTask)).Returns(familyTask);
        A.CallTo(() => mockMapper.MapToViewModel(familyTask)).Returns(expectedViewModel);

        var service = new FamilyTaskService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.CreateTaskAsync(request, creatorUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Book venue", result.Title);
        
        // Verify notifications were created for both creator and assigned user
        A.CallTo(() => mockNotificationService.CreateNotificationAsync(
            A<string>._,
            "Task",
            A<string>._,
            A<string>._,
            A<string>._,
            A<int>._,
            A<string>._)).MustHaveHappened(2, Times.Exactly);
    }

    [Fact]
    public async Task UpdateTaskAsync_AllowsCreatorToUpdate()
    {
        // Arrange
        var taskId = 1;
        var userId = "user1";
        var familyTask = new FamilyTask
        {
            Id = taskId,
            Title = "Original Title",
            CreatedByUserId = userId,
            AssignedToUserId = userId
        };

        var request = new UpdateFamilyTaskRequest
        {
            Id = taskId,
            Title = "Updated Title",
            Status = "InProgress"
        };

        var expectedViewModel = new FamilyTaskViewModel
        {
            Id = taskId,
            Title = "Updated Title"
        };

        var mockRepository = A.Fake<IFamilyTaskRepository>();
        var mockMapper = A.Fake<IFamilyTaskMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByIdAsync(taskId)).Returns(familyTask);
        A.CallTo(() => mockRepository.UpdateAsync(familyTask)).Returns(familyTask);
        A.CallTo(() => mockMapper.MapToViewModel(familyTask)).Returns(expectedViewModel);

        var service = new FamilyTaskService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.UpdateTaskAsync(taskId, request, userId);

        // Assert
        Assert.NotNull(result);
        A.CallTo(() => mockMapper.UpdateEntity(familyTask, request)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetByStatusAsync_ReturnsTasksWithGivenStatus()
    {
        // Arrange
        var tasks = new List<FamilyTask>
        {
            new FamilyTask { Id = 1, Title = "Task 1", Status = "Completed" },
            new FamilyTask { Id = 2, Title = "Task 2", Status = "Completed" }
        };

        var viewModels = new List<FamilyTaskViewModel>
        {
            new FamilyTaskViewModel { Id = 1, Title = "Task 1" },
            new FamilyTaskViewModel { Id = 2, Title = "Task 2" }
        };

        var mockRepository = A.Fake<IFamilyTaskRepository>();
        var mockMapper = A.Fake<IFamilyTaskMapper>();
        var mockNotificationService = A.Fake<INotificationService>();

        A.CallTo(() => mockRepository.GetByStatusAsync("Completed")).Returns(tasks);
        A.CallTo(() => mockMapper.MapToViewModel(tasks[0])).Returns(viewModels[0]);
        A.CallTo(() => mockMapper.MapToViewModel(tasks[1])).Returns(viewModels[1]);

        var service = new FamilyTaskService(mockRepository, mockMapper, mockNotificationService);

        // Act
        var result = await service.GetByStatusAsync("Completed");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
