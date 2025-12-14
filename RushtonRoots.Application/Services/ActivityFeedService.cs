using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class ActivityFeedService : IActivityFeedService
{
    private readonly IActivityFeedRepository _activityRepository;

    public ActivityFeedService(IActivityFeedRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<IEnumerable<ActivityFeedItemViewModel>> GetRecentActivitiesAsync(int count = 50)
    {
        var activities = await _activityRepository.GetRecentActivitiesAsync(count);
        return activities.Select(MapToViewModel);
    }

    public async Task<IEnumerable<ActivityFeedItemViewModel>> GetUserActivitiesAsync(string userId, int count = 50)
    {
        var activities = await _activityRepository.GetUserActivitiesAsync(userId, count);
        return activities.Select(MapToViewModel);
    }

    public async Task<IEnumerable<ActivityFeedItemViewModel>> GetPublicActivitiesAsync(int count = 50)
    {
        var activities = await _activityRepository.GetPublicActivitiesAsync(count);
        return activities.Select(MapToViewModel);
    }

    public async Task RecordActivityAsync(string userId, string activityType, string? entityType, int? entityId, string description, int points)
    {
        var activity = new ActivityFeedItem
        {
            UserId = userId,
            ActivityType = activityType,
            EntityType = entityType,
            EntityId = entityId,
            Description = description,
            Points = points,
            IsPublic = true
        };

        await _activityRepository.CreateAsync(activity);
    }

    private ActivityFeedItemViewModel MapToViewModel(ActivityFeedItem item)
    {
        return new ActivityFeedItemViewModel
        {
            Id = item.Id,
            UserId = item.UserId,
            UserName = item.User?.UserName ?? "Unknown",
            ActivityType = item.ActivityType,
            EntityType = item.EntityType,
            EntityId = item.EntityId,
            Description = item.Description,
            ActionUrl = item.ActionUrl,
            Points = item.Points,
            IsPublic = item.IsPublic,
            CreatedDateTime = item.CreatedDateTime
        };
    }
}
