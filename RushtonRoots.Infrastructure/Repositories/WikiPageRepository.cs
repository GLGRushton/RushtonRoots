using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class WikiPageRepository : IWikiPageRepository
{
    private readonly RushtonRootsDbContext _context;

    public WikiPageRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<WikiPage?> GetByIdAsync(int id)
    {
        return await _context.WikiPages
            .Include(w => w.Category)
            .Include(w => w.Template)
            .Include(w => w.Tags)
            .Include(w => w.CreatedByUser)
            .Include(w => w.LastUpdatedByUser)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WikiPage?> GetBySlugAsync(string slug)
    {
        return await _context.WikiPages
            .Include(w => w.Category)
            .Include(w => w.Template)
            .Include(w => w.Tags)
            .Include(w => w.CreatedByUser)
            .Include(w => w.LastUpdatedByUser)
            .FirstOrDefaultAsync(w => w.Slug == slug);
    }

    public async Task<List<WikiPage>> GetAllAsync(bool publishedOnly = false)
    {
        var query = _context.WikiPages
            .Include(w => w.Category)
            .Include(w => w.Tags)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(w => w.IsPublished);
        }

        return await query.OrderBy(w => w.Title).ToListAsync();
    }

    public async Task<List<WikiPage>> SearchAsync(string? searchTerm, int? categoryId, List<int>? tagIds, bool? isPublished, string? sortBy, bool sortDescending, int pageNumber, int pageSize)
    {
        var query = _context.WikiPages
            .Include(w => w.Category)
            .Include(w => w.Tags)
            .Include(w => w.CreatedByUser)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(w => w.Title.Contains(searchTerm) || w.Content.Contains(searchTerm) || (w.Summary != null && w.Summary.Contains(searchTerm)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(w => w.CategoryId == categoryId.Value);
        }

        if (tagIds != null && tagIds.Any())
        {
            query = query.Where(w => w.Tags.Any(t => tagIds.Contains(t.Id)));
        }

        if (isPublished.HasValue)
        {
            query = query.Where(w => w.IsPublished == isPublished.Value);
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "title" => sortDescending ? query.OrderByDescending(w => w.Title) : query.OrderBy(w => w.Title),
            "createddate" => sortDescending ? query.OrderByDescending(w => w.CreatedDateTime) : query.OrderBy(w => w.CreatedDateTime),
            "updateddate" => sortDescending ? query.OrderByDescending(w => w.UpdatedDateTime) : query.OrderBy(w => w.UpdatedDateTime),
            "viewcount" => sortDescending ? query.OrderByDescending(w => w.ViewCount) : query.OrderBy(w => w.ViewCount),
            _ => query.OrderByDescending(w => w.UpdatedDateTime)
        };

        // Apply pagination
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetSearchCountAsync(string? searchTerm, int? categoryId, List<int>? tagIds, bool? isPublished)
    {
        var query = _context.WikiPages.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(w => w.Title.Contains(searchTerm) || w.Content.Contains(searchTerm) || (w.Summary != null && w.Summary.Contains(searchTerm)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(w => w.CategoryId == categoryId.Value);
        }

        if (tagIds != null && tagIds.Any())
        {
            query = query.Where(w => w.Tags.Any(t => tagIds.Contains(t.Id)));
        }

        if (isPublished.HasValue)
        {
            query = query.Where(w => w.IsPublished == isPublished.Value);
        }

        return await query.CountAsync();
    }

    public async Task<WikiPage> AddAsync(WikiPage wikiPage)
    {
        _context.WikiPages.Add(wikiPage);
        await _context.SaveChangesAsync();
        return wikiPage;
    }

    public async Task<WikiPage> UpdateAsync(WikiPage wikiPage)
    {
        _context.WikiPages.Update(wikiPage);
        await _context.SaveChangesAsync();
        return wikiPage;
    }

    public async Task DeleteAsync(int id)
    {
        var wikiPage = await _context.WikiPages.FindAsync(id);
        if (wikiPage != null)
        {
            _context.WikiPages.Remove(wikiPage);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.WikiPages.Where(w => w.Slug == slug);
        if (excludeId.HasValue)
        {
            query = query.Where(w => w.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task IncrementViewCountAsync(int id)
    {
        var wikiPage = await _context.WikiPages.FindAsync(id);
        if (wikiPage != null)
        {
            wikiPage.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<WikiPage>> GetByCategoryAsync(int categoryId)
    {
        return await _context.WikiPages
            .Include(w => w.Tags)
            .Where(w => w.CategoryId == categoryId && w.IsPublished)
            .OrderBy(w => w.Title)
            .ToListAsync();
    }

    public async Task<List<WikiPage>> GetByTagAsync(int tagId)
    {
        return await _context.WikiPages
            .Include(w => w.Category)
            .Include(w => w.Tags)
            .Where(w => w.Tags.Any(t => t.Id == tagId) && w.IsPublished)
            .OrderBy(w => w.Title)
            .ToListAsync();
    }

    public async Task<List<WikiPage>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var query = _context.WikiPages
            .Include(w => w.Category)
            .Include(w => w.Tags)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(w => w.IsPublished);
        }

        return await query
            .OrderByDescending(w => w.UpdatedDateTime)
            .Take(count)
            .ToListAsync();
    }
}
