using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using System.Text.RegularExpressions;

namespace RushtonRoots.Application.Services;

/// <summary>
/// Service interface for StoryCollection operations
/// </summary>
public interface IStoryCollectionService
{
    Task<StoryCollectionViewModel?> GetByIdAsync(int id, bool includeStories = false);
    Task<StoryCollectionViewModel?> GetBySlugAsync(string slug, bool includeStories = false);
    Task<IEnumerable<StoryCollectionViewModel>> GetAllAsync(bool publishedOnly = false);
    Task<StoryCollectionViewModel> CreateAsync(CreateStoryCollectionRequest request, string userId);
    Task<StoryCollectionViewModel> UpdateAsync(int id, UpdateStoryCollectionRequest request);
    Task DeleteAsync(int id);
}

/// <summary>
/// Service implementation for StoryCollection operations
/// </summary>
public class StoryCollectionService : IStoryCollectionService
{
    private readonly IStoryCollectionRepository _collectionRepository;
    private readonly RushtonRoots.Infrastructure.Database.RushtonRootsDbContext _context;

    public StoryCollectionService(IStoryCollectionRepository collectionRepository, RushtonRoots.Infrastructure.Database.RushtonRootsDbContext context)
    {
        _collectionRepository = collectionRepository;
        _context = context;
    }

    public async Task<StoryCollectionViewModel?> GetByIdAsync(int id, bool includeStories = false)
    {
        var collection = await _collectionRepository.GetByIdAsync(id, includeStories);
        return collection == null ? null : MapToViewModel(collection);
    }

    public async Task<StoryCollectionViewModel?> GetBySlugAsync(string slug, bool includeStories = false)
    {
        var collection = await _collectionRepository.GetBySlugAsync(slug, includeStories);
        return collection == null ? null : MapToViewModel(collection);
    }

    public async Task<IEnumerable<StoryCollectionViewModel>> GetAllAsync(bool publishedOnly = false)
    {
        var collections = await _collectionRepository.GetAllAsync(publishedOnly);
        return collections.Select(MapToViewModel);
    }

    public async Task<StoryCollectionViewModel> CreateAsync(CreateStoryCollectionRequest request, string userId)
    {
        var collection = new StoryCollection
        {
            Name = request.Name,
            Slug = await GenerateUniqueSlugAsync(request.Name),
            Description = request.Description,
            CoverImageUrl = request.CoverImageUrl,
            CreatedByUserId = userId,
            IsPublished = request.IsPublished,
            DisplayOrder = request.DisplayOrder
        };

        var created = await _collectionRepository.AddAsync(collection);
        
        // Reload with related data
        var fullCollection = await _collectionRepository.GetByIdAsync(created.Id);
        return MapToViewModel(fullCollection!);
    }

    public async Task<StoryCollectionViewModel> UpdateAsync(int id, UpdateStoryCollectionRequest request)
    {
        var collection = await _collectionRepository.GetByIdAsync(id);
        if (collection == null)
        {
            throw new InvalidOperationException($"Story collection with ID {id} not found");
        }

        collection.Name = request.Name;
        collection.Slug = await GenerateUniqueSlugAsync(request.Name, id);
        collection.Description = request.Description;
        collection.CoverImageUrl = request.CoverImageUrl;
        collection.IsPublished = request.IsPublished;
        collection.DisplayOrder = request.DisplayOrder;

        var updated = await _collectionRepository.UpdateAsync(collection);
        
        // Reload with related data
        var fullCollection = await _collectionRepository.GetByIdAsync(updated.Id);
        return MapToViewModel(fullCollection!);
    }

    public async Task DeleteAsync(int id)
    {
        await _collectionRepository.DeleteAsync(id);
    }

    private StoryCollectionViewModel MapToViewModel(StoryCollection collection)
    {
        return new StoryCollectionViewModel
        {
            Id = collection.Id,
            Name = collection.Name,
            Slug = collection.Slug,
            Description = collection.Description,
            CoverImageUrl = collection.CoverImageUrl,
            CreatedByUserId = collection.CreatedByUserId,
            CreatedByUserName = collection.CreatedByUser?.UserName,
            IsPublished = collection.IsPublished,
            DisplayOrder = collection.DisplayOrder,
            CreatedDateTime = collection.CreatedDateTime,
            UpdatedDateTime = collection.UpdatedDateTime,
            StoryCount = collection.Stories?.Count ?? 0
        };
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, int? excludeId = null)
    {
        var baseSlug = Regex.Replace(name.ToLower(), @"[^a-z0-9\s-]", "");
        baseSlug = Regex.Replace(baseSlug, @"\s+", "-");
        baseSlug = Regex.Replace(baseSlug, @"-+", "-");
        baseSlug = baseSlug.Trim('-');

        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "collection";
        }

        var slug = baseSlug;
        var counter = 1;

        while (true)
        {
            var query = _context.StoryCollections.Where(c => c.Slug == slug);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
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
