using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using System.Text.RegularExpressions;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for Tradition operations
/// </summary>
public interface ITraditionService
{
    Task<TraditionViewModel?> GetByIdAsync(int id, bool incrementViewCount = false);
    Task<TraditionViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false);
    Task<IEnumerable<TraditionViewModel>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<TraditionViewModel>> GetByCategoryAsync(string category, bool publishedOnly = true);
    Task<IEnumerable<TraditionViewModel>> GetByStatusAsync(string status, bool publishedOnly = true);
    Task<IEnumerable<TraditionViewModel>> GetByPersonAsync(int personId, bool publishedOnly = true);
    Task<IEnumerable<TraditionViewModel>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<TraditionSearchResult> SearchAsync(SearchTraditionRequest request);
    Task<TraditionViewModel> CreateAsync(CreateTraditionRequest request, string userId);
    Task<TraditionViewModel> UpdateAsync(int id, UpdateTraditionRequest request, string userId);
    Task DeleteAsync(int id);
    Task<IEnumerable<string>> GetCategoriesAsync();
}

/// <summary>
/// Service interface for TraditionTimeline operations
/// </summary>
public interface ITraditionTimelineService
{
    Task<TraditionTimelineViewModel?> GetByIdAsync(int id);
    Task<IEnumerable<TraditionTimelineViewModel>> GetByTraditionAsync(int traditionId);
    Task<TraditionTimelineViewModel> CreateAsync(CreateTraditionTimelineRequest request, string userId);
    Task<TraditionTimelineViewModel> UpdateAsync(int id, UpdateTraditionTimelineRequest request, string userId);
    Task DeleteAsync(int id);
}

/// <summary>
/// Service implementation for Tradition operations
/// </summary>
public class TraditionService : ITraditionService
{
    private readonly ITraditionRepository _traditionRepository;
    private readonly RushtonRoots.Infrastructure.Database.RushtonRootsDbContext _context;

    public TraditionService(ITraditionRepository traditionRepository, RushtonRoots.Infrastructure.Database.RushtonRootsDbContext context)
    {
        _traditionRepository = traditionRepository;
        _context = context;
    }

    public async Task<TraditionViewModel?> GetByIdAsync(int id, bool incrementViewCount = false)
    {
        var tradition = await _traditionRepository.GetByIdAsync(id, includeRelated: true);
        if (tradition == null) return null;

        if (incrementViewCount)
        {
            await _traditionRepository.IncrementViewCountAsync(id);
            tradition.ViewCount++;
        }

        return MapToViewModel(tradition);
    }

    public async Task<TraditionViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false)
    {
        var tradition = await _traditionRepository.GetBySlugAsync(slug, includeRelated: true);
        if (tradition == null) return null;

        if (incrementViewCount)
        {
            await _traditionRepository.IncrementViewCountAsync(tradition.Id);
            tradition.ViewCount++;
        }

        return MapToViewModel(tradition);
    }

    public async Task<IEnumerable<TraditionViewModel>> GetAllAsync(bool publishedOnly = false)
    {
        var traditions = await _traditionRepository.GetAllAsync(publishedOnly);
        return traditions.Select(MapToViewModel);
    }

    public async Task<IEnumerable<TraditionViewModel>> GetByCategoryAsync(string category, bool publishedOnly = true)
    {
        var traditions = await _traditionRepository.GetByCategoryAsync(category, publishedOnly);
        return traditions.Select(MapToViewModel);
    }

    public async Task<IEnumerable<TraditionViewModel>> GetByStatusAsync(string status, bool publishedOnly = true)
    {
        var traditions = await _traditionRepository.GetByStatusAsync(status, publishedOnly);
        return traditions.Select(MapToViewModel);
    }

    public async Task<IEnumerable<TraditionViewModel>> GetByPersonAsync(int personId, bool publishedOnly = true)
    {
        var traditions = await _traditionRepository.GetByPersonAsync(personId, publishedOnly);
        return traditions.Select(MapToViewModel);
    }

    public async Task<IEnumerable<TraditionViewModel>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var traditions = await _traditionRepository.GetRecentAsync(count, publishedOnly);
        return traditions.Select(MapToViewModel);
    }

    public async Task<TraditionSearchResult> SearchAsync(SearchTraditionRequest request)
    {
        var query = _context.Traditions
            .Include(t => t.SubmittedByUser)
            .Include(t => t.StartedByPerson)
            .Include(t => t.Timeline)
            .AsQueryable();

        // Apply filters
        if (request.IsPublished.HasValue)
        {
            query = query.Where(t => t.IsPublished == request.IsPublished.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(searchTerm) ||
                t.Description.ToLower().Contains(searchTerm) ||
                (t.HowToCelebrate != null && t.HowToCelebrate.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(t => t.Category == request.Category);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(t => t.Status == request.Status);
        }

        if (request.StartedByPersonId.HasValue)
        {
            query = query.Where(t => t.StartedByPersonId == request.StartedByPersonId.Value);
        }

        // Get total count before sorting and paging
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = request.SortBy.ToLower() switch
        {
            "starteddate" => request.SortDescending
                ? query.OrderByDescending(t => t.StartedDate)
                : query.OrderBy(t => t.StartedDate),
            "createddatetime" => request.SortDescending
                ? query.OrderByDescending(t => t.CreatedDateTime)
                : query.OrderBy(t => t.CreatedDateTime),
            "updateddatetime" => request.SortDescending
                ? query.OrderByDescending(t => t.UpdatedDateTime)
                : query.OrderBy(t => t.UpdatedDateTime),
            _ => request.SortDescending
                ? query.OrderByDescending(t => t.Name)
                : query.OrderBy(t => t.Name)
        };

        // Apply paging
        var traditions = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new TraditionSearchResult
        {
            Traditions = traditions.Select(MapToViewModel).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<TraditionViewModel> CreateAsync(CreateTraditionRequest request, string userId)
    {
        var tradition = new Tradition
        {
            Name = request.Name,
            Slug = await GenerateUniqueSlugAsync(request.Name),
            Description = request.Description,
            Category = request.Category,
            Frequency = request.Frequency,
            StartedDate = request.StartedDate,
            StartedByPersonId = request.StartedByPersonId,
            Status = request.Status,
            PhotoUrl = request.PhotoUrl,
            HowToCelebrate = request.HowToCelebrate,
            AssociatedItems = request.AssociatedItems,
            SubmittedByUserId = userId,
            IsPublished = request.IsPublished,
            ViewCount = 0
        };

        await _traditionRepository.AddAsync(tradition);

        // Reload with related data
        tradition = await _traditionRepository.GetByIdAsync(tradition.Id, includeRelated: true);
        return MapToViewModel(tradition!);
    }

    public async Task<TraditionViewModel> UpdateAsync(int id, UpdateTraditionRequest request, string userId)
    {
        var tradition = await _traditionRepository.GetByIdAsync(id);
        if (tradition == null)
            throw new InvalidOperationException($"Tradition with ID {id} not found");

        tradition.Name = request.Name;
        tradition.Slug = await GenerateUniqueSlugAsync(request.Name, id);
        tradition.Description = request.Description;
        tradition.Category = request.Category;
        tradition.Frequency = request.Frequency;
        tradition.StartedDate = request.StartedDate;
        tradition.StartedByPersonId = request.StartedByPersonId;
        tradition.Status = request.Status;
        tradition.PhotoUrl = request.PhotoUrl;
        tradition.HowToCelebrate = request.HowToCelebrate;
        tradition.AssociatedItems = request.AssociatedItems;
        tradition.IsPublished = request.IsPublished;

        await _traditionRepository.UpdateAsync(tradition);

        // Reload with related data
        tradition = await _traditionRepository.GetByIdAsync(tradition.Id, includeRelated: true);
        return MapToViewModel(tradition!);
    }

    public async Task DeleteAsync(int id)
    {
        await _traditionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await _context.Traditions
            .Where(t => t.IsPublished)
            .Select(t => t.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null)
    {
        var baseSlug = Regex.Replace(name.ToLower(), @"[^a-z0-9\s-]", "");
        baseSlug = Regex.Replace(baseSlug, @"\s+", "-").Trim('-');

        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var existing = await _context.Traditions
                .Where(t => t.Slug == slug && (!excludeId.HasValue || t.Id != excludeId.Value))
                .AnyAsync();

            if (!existing)
                break;

            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    private TraditionViewModel MapToViewModel(Tradition tradition)
    {
        return new TraditionViewModel
        {
            Id = tradition.Id,
            Name = tradition.Name,
            Slug = tradition.Slug,
            Description = tradition.Description,
            Category = tradition.Category,
            Frequency = tradition.Frequency,
            StartedDate = tradition.StartedDate,
            StartedByPersonId = tradition.StartedByPersonId,
            StartedByPersonName = tradition.StartedByPerson != null
                ? $"{tradition.StartedByPerson.FirstName} {tradition.StartedByPerson.LastName}"
                : null,
            Status = tradition.Status,
            PhotoUrl = tradition.PhotoUrl,
            HowToCelebrate = tradition.HowToCelebrate,
            AssociatedItems = tradition.AssociatedItems,
            SubmittedByUserId = tradition.SubmittedByUserId,
            SubmittedByUserName = tradition.SubmittedByUser?.UserName,
            IsPublished = tradition.IsPublished,
            ViewCount = tradition.ViewCount,
            CreatedDateTime = tradition.CreatedDateTime,
            UpdatedDateTime = tradition.UpdatedDateTime,
            Timeline = tradition.Timeline?.OrderBy(t => t.EventDate).Select(MapTimelineToViewModel).ToList() ?? new List<TraditionTimelineViewModel>()
        };
    }

    private TraditionTimelineViewModel MapTimelineToViewModel(TraditionTimeline timeline)
    {
        return new TraditionTimelineViewModel
        {
            Id = timeline.Id,
            TraditionId = timeline.TraditionId,
            EventDate = timeline.EventDate,
            Title = timeline.Title,
            Description = timeline.Description,
            EventType = timeline.EventType,
            RecordedByUserId = timeline.RecordedByUserId,
            RecordedByUserName = timeline.RecordedByUser?.UserName,
            PhotoUrl = timeline.PhotoUrl,
            CreatedDateTime = timeline.CreatedDateTime,
            UpdatedDateTime = timeline.UpdatedDateTime
        };
    }
}

/// <summary>
/// Service implementation for TraditionTimeline operations
/// </summary>
public class TraditionTimelineService : ITraditionTimelineService
{
    private readonly ITraditionTimelineRepository _timelineRepository;

    public TraditionTimelineService(ITraditionTimelineRepository timelineRepository)
    {
        _timelineRepository = timelineRepository;
    }

    public async Task<TraditionTimelineViewModel?> GetByIdAsync(int id)
    {
        var timeline = await _timelineRepository.GetByIdAsync(id);
        return timeline != null ? MapToViewModel(timeline) : null;
    }

    public async Task<IEnumerable<TraditionTimelineViewModel>> GetByTraditionAsync(int traditionId)
    {
        var timelines = await _timelineRepository.GetByTraditionAsync(traditionId);
        return timelines.Select(MapToViewModel);
    }

    public async Task<TraditionTimelineViewModel> CreateAsync(CreateTraditionTimelineRequest request, string userId)
    {
        var timeline = new TraditionTimeline
        {
            TraditionId = request.TraditionId,
            EventDate = request.EventDate,
            Title = request.Title,
            Description = request.Description,
            EventType = request.EventType,
            RecordedByUserId = userId,
            PhotoUrl = request.PhotoUrl
        };

        await _timelineRepository.AddAsync(timeline);
        return MapToViewModel((await _timelineRepository.GetByIdAsync(timeline.Id))!);
    }

    public async Task<TraditionTimelineViewModel> UpdateAsync(int id, UpdateTraditionTimelineRequest request, string userId)
    {
        var timeline = await _timelineRepository.GetByIdAsync(id);
        if (timeline == null)
            throw new InvalidOperationException($"Timeline entry with ID {id} not found");

        timeline.EventDate = request.EventDate;
        timeline.Title = request.Title;
        timeline.Description = request.Description;
        timeline.EventType = request.EventType;
        timeline.PhotoUrl = request.PhotoUrl;

        await _timelineRepository.UpdateAsync(timeline);
        return MapToViewModel((await _timelineRepository.GetByIdAsync(id))!);
    }

    public async Task DeleteAsync(int id)
    {
        await _timelineRepository.DeleteAsync(id);
    }

    private TraditionTimelineViewModel MapToViewModel(TraditionTimeline timeline)
    {
        return new TraditionTimelineViewModel
        {
            Id = timeline.Id,
            TraditionId = timeline.TraditionId,
            EventDate = timeline.EventDate,
            Title = timeline.Title,
            Description = timeline.Description,
            EventType = timeline.EventType,
            RecordedByUserId = timeline.RecordedByUserId,
            RecordedByUserName = timeline.RecordedByUser?.UserName,
            PhotoUrl = timeline.PhotoUrl,
            CreatedDateTime = timeline.CreatedDateTime,
            UpdatedDateTime = timeline.UpdatedDateTime
        };
    }
}
