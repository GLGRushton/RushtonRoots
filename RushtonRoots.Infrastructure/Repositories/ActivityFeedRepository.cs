using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class ActivityFeedRepository : IActivityFeedRepository
{
    private readonly RushtonRootsDbContext _context;

    public ActivityFeedRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityFeedItem?> GetByIdAsync(int id)
    {
        return await _context.ActivityFeedItems
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<ActivityFeedItem>> GetRecentActivitiesAsync(int count = 50)
    {
        return await _context.ActivityFeedItems
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedDateTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityFeedItem>> GetUserActivitiesAsync(string userId, int count = 50)
    {
        return await _context.ActivityFeedItems
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedDateTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<ActivityFeedItem>> GetPublicActivitiesAsync(int count = 50)
    {
        return await _context.ActivityFeedItems
            .Include(a => a.User)
            .Where(a => a.IsPublic)
            .OrderByDescending(a => a.CreatedDateTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task<ActivityFeedItem> CreateAsync(ActivityFeedItem item)
    {
        _context.ActivityFeedItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _context.ActivityFeedItems.FindAsync(id);
        if (item != null)
        {
            _context.ActivityFeedItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
