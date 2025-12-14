using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface INotificationMapper
{
    NotificationViewModel MapToViewModel(Notification notification);
    NotificationPreferenceViewModel MapToViewModel(NotificationPreference preference);
    NotificationPreference MapToEntity(UpdateNotificationPreferenceRequest request, string userId);
    void UpdateEntity(NotificationPreference preference, UpdateNotificationPreferenceRequest request);
}
