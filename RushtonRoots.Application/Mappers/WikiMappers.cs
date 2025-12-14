using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IWikiCategoryMapper
{
    WikiCategoryViewModel MapToViewModel(WikiCategory category);
    WikiCategory MapToEntity(CreateWikiCategoryRequest request);
    void MapUpdateToEntity(UpdateWikiCategoryRequest request, WikiCategory category);
}

public class WikiCategoryMapper : IWikiCategoryMapper
{
    public WikiCategoryViewModel MapToViewModel(WikiCategory category)
    {
        return new WikiCategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            Icon = category.Icon,
            DisplayOrder = category.DisplayOrder,
            PageCount = category.WikiPages.Count,
            ChildCategories = category.ChildCategories.Select(MapToViewModel).ToList()
        };
    }

    public WikiCategory MapToEntity(CreateWikiCategoryRequest request)
    {
        return new WikiCategory
        {
            Name = request.Name,
            Slug = GenerateSlug(request.Name),
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            Icon = request.Icon,
            DisplayOrder = request.DisplayOrder
        };
    }

    public void MapUpdateToEntity(UpdateWikiCategoryRequest request, WikiCategory category)
    {
        category.Name = request.Name;
        category.Slug = GenerateSlug(request.Name);
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.Icon = request.Icon;
        category.DisplayOrder = request.DisplayOrder;
    }

    private string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");

        slug = new string(slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return slug.Trim('-');
    }
}

public interface IWikiTagMapper
{
    WikiTagViewModel MapToViewModel(WikiTag tag);
    WikiTag MapToEntity(CreateWikiTagRequest request);
}

public class WikiTagMapper : IWikiTagMapper
{
    public WikiTagViewModel MapToViewModel(WikiTag tag)
    {
        return new WikiTagViewModel
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug,
            Description = tag.Description,
            UsageCount = tag.UsageCount
        };
    }

    public WikiTag MapToEntity(CreateWikiTagRequest request)
    {
        return new WikiTag
        {
            Name = request.Name,
            Slug = GenerateSlug(request.Name),
            Description = request.Description
        };
    }

    private string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");

        slug = new string(slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return slug.Trim('-');
    }
}

public interface IWikiTemplateMapper
{
    WikiTemplateViewModel MapToViewModel(WikiTemplate template);
    WikiTemplate MapToEntity(CreateWikiTemplateRequest request);
    void MapUpdateToEntity(UpdateWikiTemplateRequest request, WikiTemplate template);
}

public class WikiTemplateMapper : IWikiTemplateMapper
{
    public WikiTemplateViewModel MapToViewModel(WikiTemplate template)
    {
        return new WikiTemplateViewModel
        {
            Id = template.Id,
            Name = template.Name,
            Description = template.Description,
            TemplateContent = template.TemplateContent,
            TemplateType = template.TemplateType,
            IsActive = template.IsActive,
            DisplayOrder = template.DisplayOrder
        };
    }

    public WikiTemplate MapToEntity(CreateWikiTemplateRequest request)
    {
        return new WikiTemplate
        {
            Name = request.Name,
            Description = request.Description,
            TemplateContent = request.TemplateContent,
            TemplateType = request.TemplateType,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder
        };
    }

    public void MapUpdateToEntity(UpdateWikiTemplateRequest request, WikiTemplate template)
    {
        template.Name = request.Name;
        template.Description = request.Description;
        template.TemplateContent = request.TemplateContent;
        template.TemplateType = request.TemplateType;
        template.IsActive = request.IsActive;
        template.DisplayOrder = request.DisplayOrder;
    }
}
