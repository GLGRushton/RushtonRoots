using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface INotificationService
{
    Task<NotificationViewModel?> GetByIdAsync(int id);
    Task<List<NotificationViewModel>> GetUserNotificationsAsync(string userId, bool includeRead = false, int pageSize = 50);
    Task<int> GetUnreadCountAsync(string userId);
    Task<NotificationViewModel> CreateNotificationAsync(string userId, string type, string title, string message, string? actionUrl = null, int? relatedEntityId = null, string? relatedEntityType = null);
    Task MarkAsReadAsync(int id, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task DeleteNotificationAsync(int id, string userId);
    
    // Notification preferences
    Task<List<NotificationPreferenceViewModel>> GetUserPreferencesAsync(string userId);
    Task<NotificationPreferenceViewModel> UpdatePreferenceAsync(string userId, UpdateNotificationPreferenceRequest request);
    Task<NotificationPreferenceViewModel> GetOrCreatePreferenceAsync(string userId, string notificationType);
}
