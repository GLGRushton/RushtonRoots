using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IWikiPageRepository
{
    Task<WikiPage?> GetByIdAsync(int id);
    Task<WikiPage?> GetBySlugAsync(string slug);
    Task<List<WikiPage>> GetAllAsync(bool publishedOnly = false);
    Task<List<WikiPage>> SearchAsync(string? searchTerm, int? categoryId, List<int>? tagIds, bool? isPublished, string? sortBy, bool sortDescending, int pageNumber, int pageSize);
    Task<int> GetSearchCountAsync(string? searchTerm, int? categoryId, List<int>? tagIds, bool? isPublished);
    Task<WikiPage> AddAsync(WikiPage wikiPage);
    Task<WikiPage> UpdateAsync(WikiPage wikiPage);
    Task DeleteAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    Task IncrementViewCountAsync(int id);
    Task<List<WikiPage>> GetByCategoryAsync(int categoryId);
    Task<List<WikiPage>> GetByTagAsync(int tagId);
    Task<List<WikiPage>> GetRecentAsync(int count, bool publishedOnly = true);
}
