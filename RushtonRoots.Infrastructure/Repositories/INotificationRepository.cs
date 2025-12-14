using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetByUserIdAsync(string userId, bool includeRead, int pageSize);
    Task<int> GetUnreadCountAsync(string userId);
    Task<Notification> AddAsync(Notification notification);
    Task<Notification> UpdateAsync(Notification notification);
    Task DeleteAsync(int id);
    Task MarkAllAsReadAsync(string userId);
    
    // Notification preferences
    Task<NotificationPreference?> GetPreferenceAsync(string userId, string notificationType);
    Task<List<NotificationPreference>> GetUserPreferencesAsync(string userId);
    Task<NotificationPreference> AddPreferenceAsync(NotificationPreference preference);
    Task<NotificationPreference> UpdatePreferenceAsync(NotificationPreference preference);
}
