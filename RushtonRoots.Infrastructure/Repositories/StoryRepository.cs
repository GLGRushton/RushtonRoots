using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Story entities
/// </summary>
public class StoryRepository : IStoryRepository
{
    private readonly RushtonRootsDbContext _context;

    public StoryRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Story?> GetByIdAsync(int id, bool includeRelated = false)
    {
        var query = _context.Stories.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(s => s.SubmittedByUser)
                .Include(s => s.Collection)
                .Include(s => s.StoryPeople)
                    .ThenInclude(sp => sp.Person);
        }

        return await query.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Story?> GetBySlugAsync(string slug, bool includeRelated = false)
    {
        var query = _context.Stories.AsQueryable();

        if (includeRelated)
        {
            query = query
                .Include(s => s.SubmittedByUser)
                .Include(s => s.Collection)
                .Include(s => s.StoryPeople)
                    .ThenInclude(sp => sp.Person);
        }

        return await query.FirstOrDefaultAsync(s => s.Slug == slug);
    }

    public async Task<IEnumerable<Story>> GetAllAsync(bool publishedOnly = false)
    {
        var query = _context.Stories
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Collection)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(s => s.IsPublished);
        }

        return await query.OrderByDescending(s => s.UpdatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByCategoryAsync(string category, bool publishedOnly = true)
    {
        var query = _context.Stories
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Collection)
            .Where(s => s.Category == category);

        if (publishedOnly)
        {
            query = query.Where(s => s.IsPublished);
        }

        return await query.OrderByDescending(s => s.UpdatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByPersonIdAsync(int personId, bool publishedOnly = true)
    {
        var query = _context.Stories
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Collection)
            .Where(s => s.StoryPeople.Any(sp => sp.PersonId == personId));

        if (publishedOnly)
        {
            query = query.Where(s => s.IsPublished);
        }

        return await query.OrderByDescending(s => s.UpdatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetByCollectionIdAsync(int collectionId, bool publishedOnly = true)
    {
        var query = _context.Stories
            .Include(s => s.SubmittedByUser)
            .Where(s => s.CollectionId == collectionId);

        if (publishedOnly)
        {
            query = query.Where(s => s.IsPublished);
        }

        return await query.OrderBy(s => s.CreatedDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Story>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var query = _context.Stories
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Collection)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(s => s.IsPublished);
        }

        return await query
            .OrderByDescending(s => s.UpdatedDateTime)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Story> AddAsync(Story story)
    {
        _context.Stories.Add(story);
        await _context.SaveChangesAsync();
        return story;
    }

    public async Task<Story> UpdateAsync(Story story)
    {
        _context.Stories.Update(story);
        await _context.SaveChangesAsync();
        return story;
    }

    public async Task DeleteAsync(int id)
    {
        var story = await _context.Stories.FindAsync(id);
        if (story != null)
        {
            _context.Stories.Remove(story);
            await _context.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var story = await _context.Stories.FindAsync(id);
        if (story != null)
        {
            story.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}

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
