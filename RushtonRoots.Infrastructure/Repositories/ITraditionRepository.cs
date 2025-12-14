using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Tradition entities
/// </summary>
public interface ITraditionRepository
{
    Task<Tradition?> GetByIdAsync(int id, bool includeRelated = false);
    Task<Tradition?> GetBySlugAsync(string slug, bool includeRelated = false);
    Task<IEnumerable<Tradition>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<Tradition>> GetByCategoryAsync(string category, bool publishedOnly = true);
    Task<IEnumerable<Tradition>> GetByStatusAsync(string status, bool publishedOnly = true);
    Task<IEnumerable<Tradition>> GetByPersonAsync(int personId, bool publishedOnly = true);
    Task<IEnumerable<Tradition>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<Tradition> AddAsync(Tradition tradition);
    Task<Tradition> UpdateAsync(Tradition tradition);
    Task DeleteAsync(int id);
    Task IncrementViewCountAsync(int id);
}

/// <summary>
/// Repository interface for TraditionTimeline entities
/// </summary>
public interface ITraditionTimelineRepository
{
    Task<TraditionTimeline?> GetByIdAsync(int id);
    Task<IEnumerable<TraditionTimeline>> GetByTraditionAsync(int traditionId);
    Task<TraditionTimeline> AddAsync(TraditionTimeline timelineEntry);
    Task<TraditionTimeline> UpdateAsync(TraditionTimeline timelineEntry);
    Task DeleteAsync(int id);
}
