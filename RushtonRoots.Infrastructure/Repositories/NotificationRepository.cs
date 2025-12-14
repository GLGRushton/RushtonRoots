using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly RushtonRootsDbContext _context;

    public NotificationRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<List<Notification>> GetByUserIdAsync(string userId, bool includeRead, int pageSize)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedDateTime)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();
    }

    public async Task<Notification> AddAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(notification.Id))!;
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
        
        // Reload with includes
        return (await GetByIdAsync(notification.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<NotificationPreference?> GetPreferenceAsync(string userId, string notificationType)
    {
        return await _context.NotificationPreferences
            .FirstOrDefaultAsync(np => np.UserId == userId && np.NotificationType == notificationType);
    }

    public async Task<List<NotificationPreference>> GetUserPreferencesAsync(string userId)
    {
        return await _context.NotificationPreferences
            .Where(np => np.UserId == userId)
            .ToListAsync();
    }

    public async Task<NotificationPreference> AddPreferenceAsync(NotificationPreference preference)
    {
        _context.NotificationPreferences.Add(preference);
        await _context.SaveChangesAsync();
        return preference;
    }

    public async Task<NotificationPreference> UpdatePreferenceAsync(NotificationPreference preference)
    {
        _context.NotificationPreferences.Update(preference);
        await _context.SaveChangesAsync();
        return preference;
    }
}
