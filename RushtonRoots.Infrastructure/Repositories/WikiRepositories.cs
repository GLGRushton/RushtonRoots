using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class WikiCategoryRepository : IWikiCategoryRepository
{
    private readonly RushtonRootsDbContext _context;

    public WikiCategoryRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<WikiCategory?> GetByIdAsync(int id)
    {
        return await _context.WikiCategories
            .Include(c => c.ParentCategory)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<WikiCategory?> GetBySlugAsync(string slug)
    {
        return await _context.WikiCategories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<List<WikiCategory>> GetAllAsync()
    {
        return await _context.WikiCategories
            .Include(c => c.ParentCategory)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<WikiCategory>> GetRootCategoriesAsync()
    {
        return await _context.WikiCategories
            .Include(c => c.ChildCategories)
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<WikiCategory> AddAsync(WikiCategory category)
    {
        _context.WikiCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<WikiCategory> UpdateAsync(WikiCategory category)
    {
        _context.WikiCategories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.WikiCategories.FindAsync(id);
        if (category != null)
        {
            _context.WikiCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.WikiCategories.Where(c => c.Slug == slug);
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }
}

public class WikiTagRepository : IWikiTagRepository
{
    private readonly RushtonRootsDbContext _context;

    public WikiTagRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<WikiTag?> GetByIdAsync(int id)
    {
        return await _context.WikiTags.FindAsync(id);
    }

    public async Task<WikiTag?> GetBySlugAsync(string slug)
    {
        return await _context.WikiTags.FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public async Task<List<WikiTag>> GetAllAsync()
    {
        return await _context.WikiTags
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<WikiTag>> GetPopularTagsAsync(int count)
    {
        return await _context.WikiTags
            .OrderByDescending(t => t.UsageCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<WikiTag> AddAsync(WikiTag tag)
    {
        _context.WikiTags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<WikiTag> UpdateAsync(WikiTag tag)
    {
        _context.WikiTags.Update(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _context.WikiTags.FindAsync(id);
        if (tag != null)
        {
            _context.WikiTags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.WikiTags.Where(t => t.Slug == slug);
        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    public async Task UpdateUsageCountAsync(int tagId)
    {
        var tag = await _context.WikiTags.FindAsync(tagId);
        if (tag != null)
        {
            tag.UsageCount = await _context.WikiPages.CountAsync(w => w.Tags.Any(t => t.Id == tagId));
            await _context.SaveChangesAsync();
        }
    }
}

public class WikiTemplateRepository : IWikiTemplateRepository
{
    private readonly RushtonRootsDbContext _context;

    public WikiTemplateRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<WikiTemplate?> GetByIdAsync(int id)
    {
        return await _context.WikiTemplates.FindAsync(id);
    }

    public async Task<List<WikiTemplate>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.WikiTemplates.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<List<WikiTemplate>> GetByTypeAsync(string templateType)
    {
        return await _context.WikiTemplates
            .Where(t => t.TemplateType == templateType && t.IsActive)
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<WikiTemplate> AddAsync(WikiTemplate template)
    {
        _context.WikiTemplates.Add(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task<WikiTemplate> UpdateAsync(WikiTemplate template)
    {
        _context.WikiTemplates.Update(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task DeleteAsync(int id)
    {
        var template = await _context.WikiTemplates.FindAsync(id);
        if (template != null)
        {
            _context.WikiTemplates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }
}

public class WikiPageVersionRepository : IWikiPageVersionRepository
{
    private readonly RushtonRootsDbContext _context;

    public WikiPageVersionRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<WikiPageVersion?> GetByIdAsync(int id)
    {
        return await _context.WikiPageVersions
            .Include(v => v.UpdatedByUser)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<WikiPageVersion>> GetByWikiPageIdAsync(int wikiPageId)
    {
        return await _context.WikiPageVersions
            .Include(v => v.UpdatedByUser)
            .Where(v => v.WikiPageId == wikiPageId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<WikiPageVersion?> GetVersionAsync(int wikiPageId, int versionNumber)
    {
        return await _context.WikiPageVersions
            .Include(v => v.UpdatedByUser)
            .FirstOrDefaultAsync(v => v.WikiPageId == wikiPageId && v.VersionNumber == versionNumber);
    }

    public async Task<WikiPageVersion> AddAsync(WikiPageVersion version)
    {
        _context.WikiPageVersions.Add(version);
        await _context.SaveChangesAsync();
        return version;
    }

    public async Task<int> GetNextVersionNumberAsync(int wikiPageId)
    {
        var maxVersion = await _context.WikiPageVersions
            .Where(v => v.WikiPageId == wikiPageId)
            .MaxAsync(v => (int?)v.VersionNumber);

        return (maxVersion ?? 0) + 1;
    }
}
