using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationMapper _mapper;
    private readonly IEmailService _emailService;

    public NotificationService(
        INotificationRepository notificationRepository,
        INotificationMapper mapper,
        IEmailService emailService)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<NotificationViewModel?> GetByIdAsync(int id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        return notification == null ? null : _mapper.MapToViewModel(notification);
    }

    public async Task<List<NotificationViewModel>> GetUserNotificationsAsync(string userId, bool includeRead = false, int pageSize = 50)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId, includeRead, pageSize);
        return notifications.Select(n => _mapper.MapToViewModel(n)).ToList();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId);
    }

    public async Task<NotificationViewModel> CreateNotificationAsync(
        string userId, 
        string type, 
        string title, 
        string message, 
        string? actionUrl = null, 
        int? relatedEntityId = null, 
        string? relatedEntityType = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            IsRead = false
        };
        
        var savedNotification = await _notificationRepository.AddAsync(notification);
        
        // Check user preferences and send email if enabled
        var preference = await GetOrCreatePreferenceAsync(userId, type);
        if (preference.EmailEnabled && preference.EmailFrequency == "Immediate")
        {
            try
            {
                await _emailService.SendNotificationEmailAsync(
                    savedNotification.User.Email!,
                    title,
                    message);
                
                savedNotification.EmailSent = true;
                savedNotification.EmailSentAt = DateTime.UtcNow;
                savedNotification = await _notificationRepository.UpdateAsync(savedNotification);
            }
            catch
            {
                // Log error but don't fail the notification creation
                // In production, this should be logged for monitoring
            }
        }
        
        return _mapper.MapToViewModel(savedNotification);
    }

    public async Task MarkAsReadAsync(int id, string userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            throw new InvalidOperationException("Notification not found");
        
        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You can only mark your own notifications as read");
        
        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notification);
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task DeleteNotificationAsync(int id, string userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            throw new InvalidOperationException("Notification not found");
        
        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own notifications");
        
        await _notificationRepository.DeleteAsync(id);
    }

    public async Task<List<NotificationPreferenceViewModel>> GetUserPreferencesAsync(string userId)
    {
        var preferences = await _notificationRepository.GetUserPreferencesAsync(userId);
        return preferences.Select(p => _mapper.MapToViewModel(p)).ToList();
    }

    public async Task<NotificationPreferenceViewModel> UpdatePreferenceAsync(string userId, UpdateNotificationPreferenceRequest request)
    {
        var existingPreference = await _notificationRepository.GetPreferenceAsync(userId, request.NotificationType);
        
        if (existingPreference == null)
        {
            var newPreference = _mapper.MapToEntity(request, userId);
            var savedPreference = await _notificationRepository.AddPreferenceAsync(newPreference);
            return _mapper.MapToViewModel(savedPreference);
        }
        else
        {
            _mapper.UpdateEntity(existingPreference, request);
            var updatedPreference = await _notificationRepository.UpdatePreferenceAsync(existingPreference);
            return _mapper.MapToViewModel(updatedPreference);
        }
    }

    public async Task<NotificationPreferenceViewModel> GetOrCreatePreferenceAsync(string userId, string notificationType)
    {
        var preference = await _notificationRepository.GetPreferenceAsync(userId, notificationType);
        
        if (preference == null)
        {
            // Create default preference
            preference = new NotificationPreference
            {
                UserId = userId,
                NotificationType = notificationType,
                InAppEnabled = true,
                EmailEnabled = true,
                EmailFrequency = "Immediate"
            };
            
            preference = await _notificationRepository.AddPreferenceAsync(preference);
        }
        
        return _mapper.MapToViewModel(preference);
    }
}
