using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service implementation for admin dashboard data operations.
/// </summary>
public class AdminDashboardService : IAdminDashboardService
{
    private readonly RushtonRootsDbContext _context;

    public AdminDashboardService(RushtonRootsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<AdminStatistics> GetSystemStatisticsAsync()
    {
        var statistics = new AdminStatistics
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalHouseholds = await _context.Households.CountAsync(),
            TotalPersons = await _context.People.CountAsync(p => !p.IsDeleted),
            MediaItems = await _context.PersonPhotos.CountAsync()
        };

        return statistics;
    }

    /// <inheritdoc />
    public async Task<List<RecentActivity>> GetRecentActivityAsync(int count = 20)
    {
        var activityItems = await _context.ActivityFeedItems
            .OrderByDescending(a => a.CreatedDateTime)
            .Take(count)
            .Select(a => new RecentActivity
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = string.Empty, // Will be populated from User if needed
                ActivityType = a.ActivityType,
                EntityType = a.EntityType,
                Description = a.Description,
                ActionUrl = a.ActionUrl,
                CreatedDateTime = a.CreatedDateTime
            })
            .ToListAsync();

        // Load user names separately
        if (activityItems.Any())
        {
            var userIds = activityItems.Select(a => a.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? string.Empty);

            foreach (var item in activityItems)
            {
                if (users.TryGetValue(item.UserId, out var username))
                {
                    item.UserName = username;
                }
            }
        }

        return activityItems;
    }
}
