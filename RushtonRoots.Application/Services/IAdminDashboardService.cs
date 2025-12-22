using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for admin dashboard data operations.
/// </summary>
public interface IAdminDashboardService
{
    /// <summary>
    /// Gets system statistics for the admin dashboard
    /// </summary>
    Task<AdminStatistics> GetSystemStatisticsAsync();

    /// <summary>
    /// Gets recent activity for the admin dashboard
    /// </summary>
    /// <param name="count">Number of recent activities to retrieve</param>
    Task<List<RecentActivity>> GetRecentActivityAsync(int count = 20);
}
