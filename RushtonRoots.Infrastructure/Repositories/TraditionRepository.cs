using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Tradition entities
/// </summary>
public class TraditionRepository : ITraditionRepository
{
    private readonly RushtonRootsDbContext _context;

    public TraditionRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Tradition?> GetByIdAsync(int id, bool includeRelated = false)
    {
        var query = _context.Traditions.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(t => t.SubmittedByUser)
                .Include(t => t.StartedByPerson)
                .Include(t => t.Timeline)
                    .ThenInclude(tt => tt.RecordedByUser);
        }

        return await query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tradition?> GetBySlugAsync(string slug, bool includeRelated = false)
    {
        var query = _context.Traditions.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(t => t.SubmittedByUser)
                .Include(t => t.StartedByPerson)
                .Include(t => t.Timeline)
                    .ThenInclude(tt => tt.RecordedByUser);
        }

        return await query.FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public async Task<IEnumerable<Tradition>> GetAllAsync(bool publishedOnly = false)
    {
        var query = _context.Traditions
            .Include(t => t.SubmittedByUser)
            .Include(t => t.StartedByPerson)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(t => t.IsPublished);
        }

        return await query.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<IEnumerable<Tradition>> GetByCategoryAsync(string category, bool publishedOnly = true)
    {
        var query = _context.Traditions
            .Include(t => t.SubmittedByUser)
            .Include(t => t.StartedByPerson)
            .Where(t => t.Category == category);

        if (publishedOnly)
        {
            query = query.Where(t => t.IsPublished);
        }

        return await query.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<IEnumerable<Tradition>> GetByStatusAsync(string status, bool publishedOnly = true)
    {
        var query = _context.Traditions
            .Include(t => t.SubmittedByUser)
            .Include(t => t.StartedByPerson)
            .Where(t => t.Status == status);

        if (publishedOnly)
        {
            query = query.Where(t => t.IsPublished);
        }

        return await query.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<IEnumerable<Tradition>> GetByPersonAsync(int personId, bool publishedOnly = true)
    {
        var query = _context.Traditions
            .Include(t => t.SubmittedByUser)
            .Include(t => t.StartedByPerson)
            .Where(t => t.StartedByPersonId == personId);

        if (publishedOnly)
        {
            query = query.Where(t => t.IsPublished);
        }

        return await query.OrderByDescending(t => t.StartedDate).ToListAsync();
    }

    public async Task<IEnumerable<Tradition>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var query = _context.Traditions
            .Include(t => t.SubmittedByUser)
            .Include(t => t.StartedByPerson)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(t => t.IsPublished);
        }

        return await query.OrderByDescending(t => t.CreatedDateTime).Take(count).ToListAsync();
    }

    public async Task<Tradition> AddAsync(Tradition tradition)
    {
        _context.Traditions.Add(tradition);
        await _context.SaveChangesAsync();
        return tradition;
    }

    public async Task<Tradition> UpdateAsync(Tradition tradition)
    {
        _context.Traditions.Update(tradition);
        await _context.SaveChangesAsync();
        return tradition;
    }

    public async Task DeleteAsync(int id)
    {
        var tradition = await _context.Traditions.FindAsync(id);
        if (tradition != null)
        {
            _context.Traditions.Remove(tradition);
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var tradition = await _context.Traditions.FindAsync(id);
        if (tradition != null)
        {
            tradition.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}

/// <summary>
/// Repository implementation for TraditionTimeline entities
/// </summary>
public class TraditionTimelineRepository : ITraditionTimelineRepository
{
    private readonly RushtonRootsDbContext _context;

    public TraditionTimelineRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<TraditionTimeline?> GetByIdAsync(int id)
    {
        return await _context.TraditionTimelines
            .Include(tt => tt.RecordedByUser)
            .Include(tt => tt.Tradition)
            .FirstOrDefaultAsync(tt => tt.Id == id);
    }

    public async Task<IEnumerable<TraditionTimeline>> GetByTraditionAsync(int traditionId)
    {
        return await _context.TraditionTimelines
            .Include(tt => tt.RecordedByUser)
            .Where(tt => tt.TraditionId == traditionId)
            .OrderBy(tt => tt.EventDate)
            .ToListAsync();
    }

    public async Task<TraditionTimeline> AddAsync(TraditionTimeline timelineEntry)
    {
        _context.TraditionTimelines.Add(timelineEntry);
        await _context.SaveChangesAsync();
        return timelineEntry;
    }

    public async Task<TraditionTimeline> UpdateAsync(TraditionTimeline timelineEntry)
    {
        _context.TraditionTimelines.Update(timelineEntry);
        await _context.SaveChangesAsync();
        return timelineEntry;
    }

    public async Task DeleteAsync(int id)
    {
        var timelineEntry = await _context.TraditionTimelines.FindAsync(id);
        if (timelineEntry != null)
        {
            _context.TraditionTimelines.Remove(timelineEntry);
            await _context.SaveChangesAsync();
        }
    }
}
