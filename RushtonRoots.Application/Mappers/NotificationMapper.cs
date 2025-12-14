using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class NotificationMapper : INotificationMapper
{
    public NotificationViewModel MapToViewModel(Notification notification)
    {
        return new NotificationViewModel
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            ActionUrl = notification.ActionUrl,
            RelatedEntityId = notification.RelatedEntityId,
            RelatedEntityType = notification.RelatedEntityType,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            EmailSent = notification.EmailSent,
            EmailSentAt = notification.EmailSentAt,
            CreatedDateTime = notification.CreatedDateTime
        };
    }

    public NotificationPreferenceViewModel MapToViewModel(NotificationPreference preference)
    {
        return new NotificationPreferenceViewModel
        {
            Id = preference.Id,
            UserId = preference.UserId,
            NotificationType = preference.NotificationType,
            InAppEnabled = preference.InAppEnabled,
            EmailEnabled = preference.EmailEnabled,
            EmailFrequency = preference.EmailFrequency
        };
    }

    public NotificationPreference MapToEntity(UpdateNotificationPreferenceRequest request, string userId)
    {
        return new NotificationPreference
        {
            UserId = userId,
            NotificationType = request.NotificationType,
            InAppEnabled = request.InAppEnabled,
            EmailEnabled = request.EmailEnabled,
            EmailFrequency = request.EmailFrequency
        };
    }

    public void UpdateEntity(NotificationPreference preference, UpdateNotificationPreferenceRequest request)
    {
        preference.InAppEnabled = request.InAppEnabled;
        preference.EmailEnabled = request.EmailEnabled;
        preference.EmailFrequency = request.EmailFrequency;
    }
}
