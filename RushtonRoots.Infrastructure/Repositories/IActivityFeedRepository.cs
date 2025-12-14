using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IActivityFeedRepository
{
    Task<ActivityFeedItem?> GetByIdAsync(int id);
    Task<IEnumerable<ActivityFeedItem>> GetRecentActivitiesAsync(int count = 50);
    Task<IEnumerable<ActivityFeedItem>> GetUserActivitiesAsync(string userId, int count = 50);
    Task<IEnumerable<ActivityFeedItem>> GetPublicActivitiesAsync(int count = 50);
    Task<ActivityFeedItem> CreateAsync(ActivityFeedItem item);
    Task DeleteAsync(int id);
}
