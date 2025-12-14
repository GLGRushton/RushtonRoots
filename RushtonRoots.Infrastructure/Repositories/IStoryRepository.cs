using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Story entities
/// </summary>
public interface IStoryRepository
{
    Task<Story?> GetByIdAsync(int id, bool includeRelated = false);
    Task<Story?> GetBySlugAsync(string slug, bool includeRelated = false);
    Task<IEnumerable<Story>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<Story>> GetByCategoryAsync(string category, bool publishedOnly = true);
    Task<IEnumerable<Story>> GetByPersonIdAsync(int personId, bool publishedOnly = true);
    Task<IEnumerable<Story>> GetByCollectionIdAsync(int collectionId, bool publishedOnly = true);
    Task<IEnumerable<Story>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<Story> AddAsync(Story story);
    Task<Story> UpdateAsync(Story story);
    Task DeleteAsync(int id);
    Task IncrementViewCountAsync(int id);
}

/// <summary>
/// Repository interface for StoryCollection entities
/// </summary>
public interface IStoryCollectionRepository
{
    Task<StoryCollection?> GetByIdAsync(int id, bool includeStories = false);
    Task<StoryCollection?> GetBySlugAsync(string slug, bool includeStories = false);
    Task<IEnumerable<StoryCollection>> GetAllAsync(bool publishedOnly = false);
    Task<StoryCollection> AddAsync(StoryCollection collection);
    Task<StoryCollection> UpdateAsync(StoryCollection collection);
    Task DeleteAsync(int id);
}
