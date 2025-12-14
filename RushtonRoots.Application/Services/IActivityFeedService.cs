using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Services;

public interface IActivityFeedService
{
    Task<IEnumerable<ActivityFeedItemViewModel>> GetRecentActivitiesAsync(int count = 50);
    Task<IEnumerable<ActivityFeedItemViewModel>> GetUserActivitiesAsync(string userId, int count = 50);
    Task<IEnumerable<ActivityFeedItemViewModel>> GetPublicActivitiesAsync(int count = 50);
    Task RecordActivityAsync(string userId, string activityType, string? entityType, int? entityId, string description, int points);
}
