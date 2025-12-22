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
    Task<List<RecipeViewModel>> GetRelatedRecipesAsync(int traditionId);
    Task<List<StoryViewModel>> GetRelatedStoriesAsync(int traditionId);
    Task<List<TraditionOccurrence>> GetPastOccurrencesAsync(int traditionId, int count = 5);
    Task<TraditionOccurrence?> GetNextOccurrenceAsync(int traditionId);
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

    public async Task<List<RecipeViewModel>> GetRelatedRecipesAsync(int traditionId)
    {
        // Get the tradition to check its associated items and category
        var tradition = await _context.Traditions
            .FirstOrDefaultAsync(t => t.Id == traditionId);

        if (tradition == null)
        {
            return new List<RecipeViewModel>();
        }

        // Get recipes that mention the tradition name or are in related categories
        var recipes = await _context.Recipes
            .Include(r => r.OriginatorPerson)
            .Include(r => r.SubmittedByUser)
            .Where(r => r.IsPublished &&
                (r.Name.Contains(tradition.Name) ||
                 (r.Notes != null && r.Notes.Contains(tradition.Name)) ||
                 (tradition.AssociatedItems != null && r.Name.Contains(tradition.AssociatedItems))))
            .OrderByDescending(r => r.AverageRating)
            .ThenByDescending(r => r.ViewCount)
            .ToListAsync();

        return recipes.Select(MapRecipeToViewModel).ToList();
    }

    public async Task<List<StoryViewModel>> GetRelatedStoriesAsync(int traditionId)
    {
        // Get the tradition to check its details
        var tradition = await _context.Traditions
            .Include(t => t.StartedByPerson)
            .FirstOrDefaultAsync(t => t.Id == traditionId);

        if (tradition == null)
        {
            return new List<StoryViewModel>();
        }

        // Get stories that mention the tradition or are related to the person who started it
        var stories = await _context.Stories
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Collection)
            .Include(s => s.StoryPeople)
                .ThenInclude(sp => sp.Person)
            .Where(s => s.IsPublished &&
                (s.Title.Contains(tradition.Name) ||
                 s.Content.Contains(tradition.Name) ||
                 (tradition.StartedByPersonId.HasValue &&
                  s.StoryPeople.Any(sp => sp.PersonId == tradition.StartedByPersonId.Value))))
            .OrderByDescending(s => s.ViewCount)
            .ThenByDescending(s => s.CreatedDateTime)
            .ToListAsync();

        return stories.Select(MapStoryToViewModel).ToList();
    }

    public async Task<List<TraditionOccurrence>> GetPastOccurrencesAsync(int traditionId, int count = 5)
    {
        var now = DateTime.UtcNow;

        var pastOccurrences = await _context.TraditionTimelines
            .Include(t => t.RecordedByUser)
            .Where(t => t.TraditionId == traditionId && t.EventDate <= now)
            .OrderByDescending(t => t.EventDate)
            .Take(count)
            .ToListAsync();

        return pastOccurrences.Select(MapTimelineToOccurrence).ToList();
    }

    public async Task<TraditionOccurrence?> GetNextOccurrenceAsync(int traditionId)
    {
        var now = DateTime.UtcNow;

        // Get the next scheduled occurrence from timeline
        var nextOccurrence = await _context.TraditionTimelines
            .Include(t => t.RecordedByUser)
            .Where(t => t.TraditionId == traditionId && t.EventDate > now)
            .OrderBy(t => t.EventDate)
            .FirstOrDefaultAsync();

        if (nextOccurrence != null)
        {
            return MapTimelineToOccurrence(nextOccurrence);
        }

        // If no future timeline entry exists, try to calculate based on frequency
        var tradition = await _context.Traditions.FindAsync(traditionId);
        if (tradition == null || tradition.Frequency == null)
        {
            return null;
        }

        // Get the most recent past occurrence to calculate next
        var lastOccurrence = await _context.TraditionTimelines
            .Where(t => t.TraditionId == traditionId && t.EventDate <= now)
            .OrderByDescending(t => t.EventDate)
            .FirstOrDefaultAsync();

        if (lastOccurrence == null && tradition.StartedDate.HasValue)
        {
            // No timeline entries, use started date as base
            var calculatedDate = CalculateNextOccurrence(tradition.StartedDate.Value, tradition.Frequency, now);
            if (calculatedDate.HasValue)
            {
                return new TraditionOccurrence
                {
                    Id = 0, // Calculated, not from database
                    TraditionId = traditionId,
                    EventDate = calculatedDate.Value,
                    Title = $"Next {tradition.Name}",
                    Description = $"Calculated next occurrence based on {tradition.Frequency} frequency",
                    EventType = "Calculated",
                    RecordedByUserId = string.Empty,
                    RecordedByUserName = null,
                    PhotoUrl = tradition.PhotoUrl,
                    CreatedDateTime = DateTime.UtcNow,
                    UpdatedDateTime = DateTime.UtcNow
                };
            }
        }
        else if (lastOccurrence != null)
        {
            var calculatedDate = CalculateNextOccurrence(lastOccurrence.EventDate, tradition.Frequency, now);
            if (calculatedDate.HasValue)
            {
                return new TraditionOccurrence
                {
                    Id = 0, // Calculated, not from database
                    TraditionId = traditionId,
                    EventDate = calculatedDate.Value,
                    Title = $"Next {tradition.Name}",
                    Description = $"Calculated next occurrence based on {tradition.Frequency} frequency",
                    EventType = "Calculated",
                    RecordedByUserId = string.Empty,
                    RecordedByUserName = null,
                    PhotoUrl = tradition.PhotoUrl,
                    CreatedDateTime = DateTime.UtcNow,
                    UpdatedDateTime = DateTime.UtcNow
                };
            }
        }

        return null;
    }

    private DateTime? CalculateNextOccurrence(DateTime baseDate, string frequency, DateTime now)
    {
        var date = baseDate;
        
        // Keep advancing until we find a future date
        while (date <= now)
        {
            date = frequency.ToLower() switch
            {
                "yearly" or "annual" or "annually" => date.AddYears(1),
                "monthly" => date.AddMonths(1),
                "weekly" => date.AddDays(7),
                "daily" => date.AddDays(1),
                _ => date.AddYears(1) // Default to yearly
            };
        }

        return date > now ? date : null;
    }

    private TraditionOccurrence MapTimelineToOccurrence(TraditionTimeline timeline)
    {
        return new TraditionOccurrence
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

    private RecipeViewModel MapRecipeToViewModel(Recipe recipe)
    {
        return new RecipeViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Slug = recipe.Slug,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            Servings = recipe.Servings,
            Category = recipe.Category,
            Cuisine = recipe.Cuisine,
            PhotoUrl = recipe.PhotoUrl,
            Notes = recipe.Notes,
            OriginatorPersonId = recipe.OriginatorPersonId,
            OriginatorPersonName = recipe.OriginatorPerson != null
                ? $"{recipe.OriginatorPerson.FirstName} {recipe.OriginatorPerson.LastName}"
                : null,
            SubmittedByUserId = recipe.SubmittedByUserId,
            SubmittedByUserName = recipe.SubmittedByUser?.UserName,
            IsPublished = recipe.IsPublished,
            IsFavorite = recipe.IsFavorite,
            AverageRating = recipe.AverageRating,
            RatingCount = recipe.RatingCount,
            ViewCount = recipe.ViewCount,
            CreatedDateTime = recipe.CreatedDateTime,
            UpdatedDateTime = recipe.UpdatedDateTime
        };
    }

    private StoryViewModel MapStoryToViewModel(Story story)
    {
        return new StoryViewModel
        {
            Id = story.Id,
            Title = story.Title,
            Slug = story.Slug,
            Content = story.Content,
            Summary = story.Summary,
            Category = story.Category,
            StoryDate = story.StoryDate,
            Location = story.Location,
            SubmittedByUserId = story.SubmittedByUserId,
            SubmittedByUserName = story.SubmittedByUser?.UserName,
            IsPublished = story.IsPublished,
            ViewCount = story.ViewCount,
            AllowCollaboration = story.AllowCollaboration,
            CollectionId = story.CollectionId,
            CollectionName = story.Collection?.Name,
            CreatedDateTime = story.CreatedDateTime,
            UpdatedDateTime = story.UpdatedDateTime,
            AssociatedPeople = story.StoryPeople.Select(sp => new PersonBasicViewModel
            {
                Id = sp.PersonId,
                FirstName = sp.Person?.FirstName ?? "",
                LastName = sp.Person?.LastName ?? "",
                RoleInStory = sp.RoleInStory
            }).ToList()
        };
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
