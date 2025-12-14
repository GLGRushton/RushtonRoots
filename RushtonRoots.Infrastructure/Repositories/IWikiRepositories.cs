using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IWikiCategoryRepository
{
    Task<WikiCategory?> GetByIdAsync(int id);
    Task<WikiCategory?> GetBySlugAsync(string slug);
    Task<List<WikiCategory>> GetAllAsync();
    Task<List<WikiCategory>> GetRootCategoriesAsync();
    Task<WikiCategory> AddAsync(WikiCategory category);
    Task<WikiCategory> UpdateAsync(WikiCategory category);
    Task DeleteAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
}

public interface IWikiTagRepository
{
    Task<WikiTag?> GetByIdAsync(int id);
    Task<WikiTag?> GetBySlugAsync(string slug);
    Task<List<WikiTag>> GetAllAsync();
    Task<List<WikiTag>> GetPopularTagsAsync(int count);
    Task<WikiTag> AddAsync(WikiTag tag);
    Task<WikiTag> UpdateAsync(WikiTag tag);
    Task DeleteAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    Task UpdateUsageCountAsync(int tagId);
}

public interface IWikiTemplateRepository
{
    Task<WikiTemplate?> GetByIdAsync(int id);
    Task<List<WikiTemplate>> GetAllAsync(bool activeOnly = false);
    Task<List<WikiTemplate>> GetByTypeAsync(string templateType);
    Task<WikiTemplate> AddAsync(WikiTemplate template);
    Task<WikiTemplate> UpdateAsync(WikiTemplate template);
    Task DeleteAsync(int id);
}

public interface IWikiPageVersionRepository
{
    Task<WikiPageVersion?> GetByIdAsync(int id);
    Task<List<WikiPageVersion>> GetByWikiPageIdAsync(int wikiPageId);
    Task<WikiPageVersion?> GetVersionAsync(int wikiPageId, int versionNumber);
    Task<WikiPageVersion> AddAsync(WikiPageVersion version);
    Task<int> GetNextVersionNumberAsync(int wikiPageId);
}
