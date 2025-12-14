using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public interface IWikiCategoryService
{
    Task<WikiCategoryViewModel?> GetByIdAsync(int id);
    Task<WikiCategoryViewModel?> GetBySlugAsync(string slug);
    Task<List<WikiCategoryViewModel>> GetAllAsync();
    Task<List<WikiCategoryViewModel>> GetRootCategoriesAsync();
    Task<WikiCategoryViewModel> CreateAsync(CreateWikiCategoryRequest request);
    Task<WikiCategoryViewModel> UpdateAsync(int id, UpdateWikiCategoryRequest request);
    Task DeleteAsync(int id);
}

public class WikiCategoryService : IWikiCategoryService
{
    private readonly IWikiCategoryRepository _repository;
    private readonly IWikiCategoryMapper _mapper;

    public WikiCategoryService(IWikiCategoryRepository repository, IWikiCategoryMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<WikiCategoryViewModel?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        return category == null ? null : _mapper.MapToViewModel(category);
    }

    public async Task<WikiCategoryViewModel?> GetBySlugAsync(string slug)
    {
        var category = await _repository.GetBySlugAsync(slug);
        return category == null ? null : _mapper.MapToViewModel(category);
    }

    public async Task<List<WikiCategoryViewModel>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => _mapper.MapToViewModel(c)).ToList();
    }

    public async Task<List<WikiCategoryViewModel>> GetRootCategoriesAsync()
    {
        var categories = await _repository.GetRootCategoriesAsync();
        return categories.Select(c => _mapper.MapToViewModel(c)).ToList();
    }

    public async Task<WikiCategoryViewModel> CreateAsync(CreateWikiCategoryRequest request)
    {
        var category = _mapper.MapToEntity(request);

        // Ensure unique slug
        var slug = category.Slug;
        var counter = 1;
        while (await _repository.SlugExistsAsync(slug))
        {
            slug = $"{category.Slug}-{counter}";
            counter++;
        }
        category.Slug = slug;

        category = await _repository.AddAsync(category);
        return _mapper.MapToViewModel(category);
    }

    public async Task<WikiCategoryViewModel> UpdateAsync(int id, UpdateWikiCategoryRequest request)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Wiki category with ID {id} not found");
        }

        _mapper.MapUpdateToEntity(request, category);

        // Ensure unique slug
        var slug = category.Slug;
        var counter = 1;
        while (await _repository.SlugExistsAsync(slug, id))
        {
            slug = $"{category.Slug}-{counter}";
            counter++;
        }
        category.Slug = slug;

        category = await _repository.UpdateAsync(category);
        return _mapper.MapToViewModel(category);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}

public interface IWikiTagService
{
    Task<WikiTagViewModel?> GetByIdAsync(int id);
    Task<WikiTagViewModel?> GetBySlugAsync(string slug);
    Task<List<WikiTagViewModel>> GetAllAsync();
    Task<List<WikiTagViewModel>> GetPopularTagsAsync(int count);
    Task<WikiTagViewModel> CreateAsync(CreateWikiTagRequest request);
    Task DeleteAsync(int id);
}

public class WikiTagService : IWikiTagService
{
    private readonly IWikiTagRepository _repository;
    private readonly IWikiTagMapper _mapper;

    public WikiTagService(IWikiTagRepository repository, IWikiTagMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<WikiTagViewModel?> GetByIdAsync(int id)
    {
        var tag = await _repository.GetByIdAsync(id);
        return tag == null ? null : _mapper.MapToViewModel(tag);
    }

    public async Task<WikiTagViewModel?> GetBySlugAsync(string slug)
    {
        var tag = await _repository.GetBySlugAsync(slug);
        return tag == null ? null : _mapper.MapToViewModel(tag);
    }

    public async Task<List<WikiTagViewModel>> GetAllAsync()
    {
        var tags = await _repository.GetAllAsync();
        return tags.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<WikiTagViewModel>> GetPopularTagsAsync(int count)
    {
        var tags = await _repository.GetPopularTagsAsync(count);
        return tags.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<WikiTagViewModel> CreateAsync(CreateWikiTagRequest request)
    {
        var tag = _mapper.MapToEntity(request);

        // Ensure unique slug
        var slug = tag.Slug;
        var counter = 1;
        while (await _repository.SlugExistsAsync(slug))
        {
            slug = $"{tag.Slug}-{counter}";
            counter++;
        }
        tag.Slug = slug;

        tag = await _repository.AddAsync(tag);
        return _mapper.MapToViewModel(tag);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}

public interface IWikiTemplateService
{
    Task<WikiTemplateViewModel?> GetByIdAsync(int id);
    Task<List<WikiTemplateViewModel>> GetAllAsync(bool activeOnly = false);
    Task<List<WikiTemplateViewModel>> GetByTypeAsync(string templateType);
    Task<WikiTemplateViewModel> CreateAsync(CreateWikiTemplateRequest request);
    Task<WikiTemplateViewModel> UpdateAsync(int id, UpdateWikiTemplateRequest request);
    Task DeleteAsync(int id);
}

public class WikiTemplateService : IWikiTemplateService
{
    private readonly IWikiTemplateRepository _repository;
    private readonly IWikiTemplateMapper _mapper;

    public WikiTemplateService(IWikiTemplateRepository repository, IWikiTemplateMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<WikiTemplateViewModel?> GetByIdAsync(int id)
    {
        var template = await _repository.GetByIdAsync(id);
        return template == null ? null : _mapper.MapToViewModel(template);
    }

    public async Task<List<WikiTemplateViewModel>> GetAllAsync(bool activeOnly = false)
    {
        var templates = await _repository.GetAllAsync(activeOnly);
        return templates.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<WikiTemplateViewModel>> GetByTypeAsync(string templateType)
    {
        var templates = await _repository.GetByTypeAsync(templateType);
        return templates.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<WikiTemplateViewModel> CreateAsync(CreateWikiTemplateRequest request)
    {
        var template = _mapper.MapToEntity(request);
        template = await _repository.AddAsync(template);
        return _mapper.MapToViewModel(template);
    }

    public async Task<WikiTemplateViewModel> UpdateAsync(int id, UpdateWikiTemplateRequest request)
    {
        var template = await _repository.GetByIdAsync(id);
        if (template == null)
        {
            throw new KeyNotFoundException($"Wiki template with ID {id} not found");
        }

        _mapper.MapUpdateToEntity(request, template);
        template = await _repository.UpdateAsync(template);
        return _mapper.MapToViewModel(template);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
