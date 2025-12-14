using Microsoft.EntityFrameworkCore;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Application.Validators;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public interface IWikiPageService
{
    Task<WikiPageViewModel?> GetByIdAsync(int id, bool incrementViewCount = false);
    Task<WikiPageViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false);
    Task<List<WikiPageViewModel>> GetAllAsync(bool publishedOnly = false);
    Task<(List<WikiPageViewModel> Pages, int TotalCount)> SearchAsync(WikiSearchRequest request);
    Task<WikiPageViewModel> CreateAsync(CreateWikiPageRequest request, string userId);
    Task<WikiPageViewModel> UpdateAsync(int id, UpdateWikiPageRequest request, string userId);
    Task DeleteAsync(int id);
    Task<List<WikiPageViewModel>> GetByCategoryAsync(int categoryId);
    Task<List<WikiPageViewModel>> GetByTagAsync(int tagId);
    Task<List<WikiPageViewModel>> GetRecentAsync(int count, bool publishedOnly = true);
    Task<List<WikiPageVersionViewModel>> GetVersionHistoryAsync(int wikiPageId);
    Task<WikiPageVersionViewModel?> GetVersionAsync(int wikiPageId, int versionNumber);
}

public class WikiPageService : IWikiPageService
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly IWikiPageVersionRepository _versionRepository;
    private readonly IWikiTagRepository _tagRepository;
    private readonly IWikiPageMapper _mapper;
    private readonly IWikiPageValidator _validator;

    public WikiPageService(
        IWikiPageRepository wikiPageRepository,
        IWikiPageVersionRepository versionRepository,
        IWikiTagRepository tagRepository,
        IWikiPageMapper mapper,
        IWikiPageValidator validator)
    {
        _wikiPageRepository = wikiPageRepository;
        _versionRepository = versionRepository;
        _tagRepository = tagRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<WikiPageViewModel?> GetByIdAsync(int id, bool incrementViewCount = false)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(id);
        if (wikiPage == null)
        {
            return null;
        }

        if (incrementViewCount)
        {
            await _wikiPageRepository.IncrementViewCountAsync(id);
            wikiPage.ViewCount++;
        }

        return _mapper.MapToViewModel(wikiPage);
    }

    public async Task<WikiPageViewModel?> GetBySlugAsync(string slug, bool incrementViewCount = false)
    {
        var wikiPage = await _wikiPageRepository.GetBySlugAsync(slug);
        if (wikiPage == null)
        {
            return null;
        }

        if (incrementViewCount)
        {
            await _wikiPageRepository.IncrementViewCountAsync(wikiPage.Id);
            wikiPage.ViewCount++;
        }

        return _mapper.MapToViewModel(wikiPage);
    }

    public async Task<List<WikiPageViewModel>> GetAllAsync(bool publishedOnly = false)
    {
        var wikiPages = await _wikiPageRepository.GetAllAsync(publishedOnly);
        return wikiPages.Select(w => _mapper.MapToViewModel(w)).ToList();
    }

    public async Task<(List<WikiPageViewModel> Pages, int TotalCount)> SearchAsync(WikiSearchRequest request)
    {
        var wikiPages = await _wikiPageRepository.SearchAsync(
            request.SearchTerm,
            request.CategoryId,
            request.TagIds,
            request.IsPublished,
            request.SortBy,
            request.SortDescending,
            request.PageNumber,
            request.PageSize);

        var totalCount = await _wikiPageRepository.GetSearchCountAsync(
            request.SearchTerm,
            request.CategoryId,
            request.TagIds,
            request.IsPublished);

        var viewModels = wikiPages.Select(w => _mapper.MapToViewModel(w)).ToList();

        return (viewModels, totalCount);
    }

    public async Task<WikiPageViewModel> CreateAsync(CreateWikiPageRequest request, string userId)
    {
        // Validate
        var validation = await _validator.ValidateCreateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(", ", validation.Errors));
        }

        // Map to entity
        var wikiPage = _mapper.MapToEntity(request, userId);

        // Ensure unique slug
        var slug = wikiPage.Slug;
        var counter = 1;
        while (await _wikiPageRepository.SlugExistsAsync(slug))
        {
            slug = $"{wikiPage.Slug}-{counter}";
            counter++;
        }
        wikiPage.Slug = slug;

        // Handle tags
        if (request.TagIds.Any())
        {
            foreach (var tagId in request.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag != null)
                {
                    wikiPage.Tags.Add(tag);
                }
            }
        }

        // Save
        wikiPage = await _wikiPageRepository.AddAsync(wikiPage);

        // Create initial version
        var version = _mapper.CreateVersion(wikiPage, userId, request.ChangeDescription ?? "Initial version");
        version.VersionNumber = 1;
        await _versionRepository.AddAsync(version);

        // Update tag usage counts
        foreach (var tagId in request.TagIds)
        {
            await _tagRepository.UpdateUsageCountAsync(tagId);
        }

        return _mapper.MapToViewModel(wikiPage);
    }

    public async Task<WikiPageViewModel> UpdateAsync(int id, UpdateWikiPageRequest request, string userId)
    {
        // Validate
        var validation = await _validator.ValidateUpdateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(", ", validation.Errors));
        }

        // Get existing
        var wikiPage = await _wikiPageRepository.GetByIdAsync(id);
        if (wikiPage == null)
        {
            throw new KeyNotFoundException($"Wiki page with ID {id} not found");
        }

        // Track old tag IDs for usage count updates
        var oldTagIds = wikiPage.Tags.Select(t => t.Id).ToList();

        // Create version before update
        var nextVersionNumber = await _versionRepository.GetNextVersionNumberAsync(id);
        var version = _mapper.CreateVersion(wikiPage, userId, request.ChangeDescription ?? $"Version {nextVersionNumber}");
        version.VersionNumber = nextVersionNumber;
        await _versionRepository.AddAsync(version);

        // Map updates
        _mapper.MapUpdateToEntity(request, wikiPage, userId);

        // Ensure unique slug
        var slug = wikiPage.Slug;
        var counter = 1;
        while (await _wikiPageRepository.SlugExistsAsync(slug, id))
        {
            slug = $"{wikiPage.Slug}-{counter}";
            counter++;
        }
        wikiPage.Slug = slug;

        // Update tags
        wikiPage.Tags.Clear();
        if (request.TagIds.Any())
        {
            foreach (var tagId in request.TagIds)
            {
                var tag = await _tagRepository.GetByIdAsync(tagId);
                if (tag != null)
                {
                    wikiPage.Tags.Add(tag);
                }
            }
        }

        // Save
        wikiPage = await _wikiPageRepository.UpdateAsync(wikiPage);

        // Update tag usage counts for all affected tags
        var allAffectedTagIds = oldTagIds.Union(request.TagIds).Distinct();
        foreach (var tagId in allAffectedTagIds)
        {
            await _tagRepository.UpdateUsageCountAsync(tagId);
        }

        return _mapper.MapToViewModel(wikiPage);
    }

    public async Task DeleteAsync(int id)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(id);
        if (wikiPage == null)
        {
            throw new KeyNotFoundException($"Wiki page with ID {id} not found");
        }

        // Track tag IDs for usage count updates
        var tagIds = wikiPage.Tags.Select(t => t.Id).ToList();

        await _wikiPageRepository.DeleteAsync(id);

        // Update tag usage counts
        foreach (var tagId in tagIds)
        {
            await _tagRepository.UpdateUsageCountAsync(tagId);
        }
    }

    public async Task<List<WikiPageViewModel>> GetByCategoryAsync(int categoryId)
    {
        var wikiPages = await _wikiPageRepository.GetByCategoryAsync(categoryId);
        return wikiPages.Select(w => _mapper.MapToViewModel(w)).ToList();
    }

    public async Task<List<WikiPageViewModel>> GetByTagAsync(int tagId)
    {
        var wikiPages = await _wikiPageRepository.GetByTagAsync(tagId);
        return wikiPages.Select(w => _mapper.MapToViewModel(w)).ToList();
    }

    public async Task<List<WikiPageViewModel>> GetRecentAsync(int count, bool publishedOnly = true)
    {
        var wikiPages = await _wikiPageRepository.GetRecentAsync(count, publishedOnly);
        return wikiPages.Select(w => _mapper.MapToViewModel(w)).ToList();
    }

    public async Task<List<WikiPageVersionViewModel>> GetVersionHistoryAsync(int wikiPageId)
    {
        var versions = await _versionRepository.GetByWikiPageIdAsync(wikiPageId);
        return versions.Select(v => _mapper.MapVersionToViewModel(v)).ToList();
    }

    public async Task<WikiPageVersionViewModel?> GetVersionAsync(int wikiPageId, int versionNumber)
    {
        var version = await _versionRepository.GetVersionAsync(wikiPageId, versionNumber);
        return version == null ? null : _mapper.MapVersionToViewModel(version);
    }
}
