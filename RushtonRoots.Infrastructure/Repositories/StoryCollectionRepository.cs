using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for StoryCollection entities
/// </summary>
public class StoryCollectionRepository : IStoryCollectionRepository
{
    private readonly RushtonRootsDbContext _context;

    public StoryCollectionRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<StoryCollection?> GetByIdAsync(int id, bool includeStories = false)
    {
        var query = _context.StoryCollections.AsQueryable();

        if (includeStories)
        {
            query = query.Include(c => c.Stories);
        }

        query = query.Include(c => c.CreatedByUser);

        return await query.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<StoryCollection?> GetBySlugAsync(string slug, bool includeStories = false)
    {
        var query = _context.StoryCollections.AsQueryable();

        if (includeStories)
        {
            query = query.Include(c => c.Stories);
        }

        query = query.Include(c => c.CreatedByUser);

        return await query.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<IEnumerable<StoryCollection>> GetAllAsync(bool publishedOnly = false)
    {
        var query = _context.StoryCollections
            .Include(c => c.CreatedByUser)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(c => c.IsPublished);
        }

        return await query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync();
    }

    public async Task<StoryCollection> AddAsync(StoryCollection collection)
    {
        _context.StoryCollections.Add(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task<StoryCollection> UpdateAsync(StoryCollection collection)
    {
        _context.StoryCollections.Update(collection);
        await _context.SaveChangesAsync();
        return collection;
    }

    public async Task DeleteAsync(int id)
    {
        var collection = await _context.StoryCollections.FindAsync(id);
        if (collection != null)
        {
            _context.StoryCollections.Remove(collection);
            await _context.SaveChangesAsync();
        }
    }
}
