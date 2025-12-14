using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using System.Text.RegularExpressions;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for Story operations
/// </summary>
public interface IStoryService
{
    Task<StoryViewModel?> GetByIdAsync(int id, bool incrementViewCount = false);
    Task<StoryViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false);
    Task<IEnumerable<StoryViewModel>> GetAllAsync(bool publishedOnly = false);
    Task<IEnumerable<StoryViewModel>> GetByCategoryAsync(string category, bool publishedOnly = true);
    Task<IEnumerable<StoryViewModel>> GetByPersonIdAsync(int personId, bool publishedOnly = true);
    Task<IEnumerable<StoryViewModel>> GetByCollectionIdAsync(int collectionId, bool publishedOnly = true);
    Task<IEnumerable<StoryViewModel>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<StorySearchResult> SearchAsync(SearchStoryRequest request);
    Task<StoryViewModel> CreateAsync(CreateStoryRequest request, string userId);
    Task<StoryViewModel> UpdateAsync(int id, UpdateStoryRequest request, string userId);
    Task DeleteAsync(int id);
    Task<IEnumerable<string>> GetCategoriesAsync();
}

/// <summary>
/// Service implementation for Story operations
/// </summary>
public class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;
    private readonly RushtonRoots.Infrastructure.Database.RushtonRootsDbContext _context;

    public StoryService(IStoryRepository storyRepository, RushtonRoots.Infrastructure.Database.RushtonRootsDbContext context)
    {
        _storyRepository = storyRepository;
        _context = context;
    }

    public async Task<StoryViewModel?> GetByIdAsync(int id, bool incrementViewCount = false)
    {
        var story = await _storyRepository.GetByIdAsync(id, includeRelated: true);
        if (story == null) return null;

        if (incrementViewCount)
        {
            await _storyRepository.IncrementViewCountAsync(id);
            story.ViewCount++; // Update local object to reflect the increment
        }

        return MapToViewModel(story);
    }

    public async Task<StoryViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false)
    {
        var story = await _storyRepository.GetBySlugAsync(slug, includeRelated: true);
        if (story == null) return null;

        if (incrementViewCount)
        {
            await _storyRepository.IncrementViewCountAsync(story.Id);
            story.ViewCount++;
        }

        return MapToViewModel(story);
    }

    public async Task<IEnumerable<StoryViewModel>> GetAllAsync(bool publishedOnly = false)
    {
        var stories = await _storyRepository.GetAllAsync(publishedOnly);
        return stories.Select(MapToViewModel);
    }

    public async Task<IEnumerable<StoryViewModel>> GetByCategoryAsync(string category, bool publishedOnly = true)
    {
        var stories = await _storyRepository.GetByCategoryAsync(category, publishedOnly);
        return stories.Select(MapToViewModel);
    }

    public async Task<IEnumerable<StoryViewModel>> GetByPersonIdAsync(int personId, bool publishedOnly = true)
    {
        var stories = await _storyRepository.GetByPersonIdAsync(personId, publishedOnly);
        return stories.Select(MapToViewModel);
    }

    public async Task<IEnumerable<StoryViewModel>> GetByCollectionIdAsync(int collectionId, bool publishedOnly = true)
    {
        var stories = await _storyRepository.GetByCollectionIdAsync(collectionId, publishedOnly);
        return stories.Select(MapToViewModel);
    }

    public async Task<IEnumerable<StoryViewModel>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var stories = await _storyRepository.GetRecentAsync(count, publishedOnly);
        return stories.Select(MapToViewModel);
    }

    public async Task<StorySearchResult> SearchAsync(SearchStoryRequest request)
    {
        var query = _context.Stories
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Collection)
            .Include(s => s.StoryPeople)
                .ThenInclude(sp => sp.Person)
            .AsQueryable();

        // Apply filters
        if (request.IsPublished.HasValue)
        {
            query = query.Where(s => s.IsPublished == request.IsPublished.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.Title.ToLower().Contains(searchTerm) ||
                s.Content.ToLower().Contains(searchTerm) ||
                (s.Summary != null && s.Summary.ToLower().Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(s => s.Category == request.Category);
        }

        if (request.PersonId.HasValue)
        {
            query = query.Where(s => s.StoryPeople.Any(sp => sp.PersonId == request.PersonId.Value));
        }

        if (request.CollectionId.HasValue)
        {
            query = query.Where(s => s.CollectionId == request.CollectionId.Value);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(s => s.StoryDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(s => s.StoryDate <= request.EndDate.Value);
        }

        // Get total count before sorting and paging
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = request.SortBy.ToLower() switch
        {
            "title" => request.SortDescending ? query.OrderByDescending(s => s.Title) : query.OrderBy(s => s.Title),
            "createddatetime" => request.SortDescending ? query.OrderByDescending(s => s.CreatedDateTime) : query.OrderBy(s => s.CreatedDateTime),
            "viewcount" => request.SortDescending ? query.OrderByDescending(s => s.ViewCount) : query.OrderBy(s => s.ViewCount),
            "storydate" => request.SortDescending ? query.OrderByDescending(s => s.StoryDate) : query.OrderBy(s => s.StoryDate),
            _ => request.SortDescending ? query.OrderByDescending(s => s.UpdatedDateTime) : query.OrderBy(s => s.UpdatedDateTime)
        };

        // Apply paging
        var stories = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new StorySearchResult
        {
            Stories = stories.Select(MapToViewModel).ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<StoryViewModel> CreateAsync(CreateStoryRequest request, string userId)
    {
        var story = new Story
        {
            Title = request.Title,
            Slug = await GenerateUniqueSlugAsync(request.Title),
            Content = request.Content,
            Summary = request.Summary,
            Category = request.Category,
            StoryDate = request.StoryDate,
            Location = request.Location,
            SubmittedByUserId = userId,
            IsPublished = request.IsPublished,
            AllowCollaboration = request.AllowCollaboration,
            CollectionId = request.CollectionId,
            ViewCount = 0
        };

        // Add person associations
        foreach (var personId in request.PersonIds)
        {
            story.StoryPeople.Add(new StoryPerson
            {
                PersonId = personId
            });
        }

        var created = await _storyRepository.AddAsync(story);
        
        // Reload with related data
        var fullStory = await _storyRepository.GetByIdAsync(created.Id, includeRelated: true);
        return MapToViewModel(fullStory!);
    }

    public async Task<StoryViewModel> UpdateAsync(int id, UpdateStoryRequest request, string userId)
    {
        var story = await _storyRepository.GetByIdAsync(id, includeRelated: true);
        if (story == null)
        {
            throw new InvalidOperationException($"Story with ID {id} not found");
        }

        // Update properties
        story.Title = request.Title;
        story.Slug = await GenerateUniqueSlugAsync(request.Title, id);
        story.Content = request.Content;
        story.Summary = request.Summary;
        story.Category = request.Category;
        story.StoryDate = request.StoryDate;
        story.Location = request.Location;
        story.IsPublished = request.IsPublished;
        story.AllowCollaboration = request.AllowCollaboration;
        story.CollectionId = request.CollectionId;

        // Update person associations
        // Remove existing associations
        var existingAssociations = _context.StoryPeople.Where(sp => sp.StoryId == id);
        _context.StoryPeople.RemoveRange(existingAssociations);

        // Add new associations
        foreach (var personId in request.PersonIds)
        {
            story.StoryPeople.Add(new StoryPerson
            {
                StoryId = id,
                PersonId = personId
            });
        }

        var updated = await _storyRepository.UpdateAsync(story);
        
        // Reload with related data
        var fullStory = await _storyRepository.GetByIdAsync(updated.Id, includeRelated: true);
        return MapToViewModel(fullStory!);
    }

    public async Task DeleteAsync(int id)
    {
        await _storyRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync()
    {
        return await _context.Stories
            .Where(s => s.IsPublished)
            .Select(s => s.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    private StoryViewModel MapToViewModel(Story story)
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

    private async Task<string> GenerateUniqueSlugAsync(string title, int? excludeId = null)
    {
        var baseSlug = Regex.Replace(title.ToLower(), @"[^a-z0-9\s-]", "");
        baseSlug = Regex.Replace(baseSlug, @"\s+", "-");
        baseSlug = Regex.Replace(baseSlug, @"-+", "-");
        baseSlug = baseSlug.Trim('-');

        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "story";
        }

        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var query = _context.Stories.Where(s => s.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            var exists = await query.AnyAsync();
            if (!exists)
            {
                break;
            }

            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }
}
