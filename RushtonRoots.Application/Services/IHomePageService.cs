using RushtonRoots.Domain.UI.Models;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for home page data operations.
/// </summary>
public interface IHomePageService
{
    /// <summary>
    /// Gets the statistics for the home page dashboard
    /// </summary>
    Task<HomePageStatistics> GetStatisticsAsync();

    /// <summary>
    /// Gets the most recent additions to the family tree
    /// </summary>
    /// <param name="count">Number of recent additions to retrieve</param>
    Task<List<RecentAddition>> GetRecentAdditionsAsync(int count = 5);

    /// <summary>
    /// Gets upcoming birthdays within the specified number of days
    /// </summary>
    /// <param name="days">Number of days ahead to check for birthdays</param>
    Task<List<UpcomingBirthday>> GetUpcomingBirthdaysAsync(int days = 30);

    /// <summary>
    /// Gets upcoming anniversaries within the specified number of days
    /// </summary>
    /// <param name="days">Number of days ahead to check for anniversaries</param>
    Task<List<UpcomingAnniversary>> GetUpcomingAnniversariesAsync(int days = 30);

    /// <summary>
    /// Gets the recent activity feed
    /// </summary>
    /// <param name="count">Number of activity items to retrieve</param>
    Task<List<ActivityFeedItemViewModel>> GetActivityFeedAsync(int count = 20);
}
