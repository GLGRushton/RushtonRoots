using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IWikiPageMapper
{
    WikiPageViewModel MapToViewModel(WikiPage wikiPage);
    WikiPage MapToEntity(CreateWikiPageRequest request, string userId);
    void MapUpdateToEntity(UpdateWikiPageRequest request, WikiPage wikiPage, string userId);
    WikiPageVersionViewModel MapVersionToViewModel(WikiPageVersion version);
    WikiPageVersion CreateVersion(WikiPage wikiPage, string userId, string? changeDescription);
}

public class WikiPageMapper : IWikiPageMapper
{
    public WikiPageViewModel MapToViewModel(WikiPage wikiPage)
    {
        return new WikiPageViewModel
        {
            Id = wikiPage.Id,
            Title = wikiPage.Title,
            Slug = wikiPage.Slug,
            Content = wikiPage.Content,
            Summary = wikiPage.Summary,
            CategoryId = wikiPage.CategoryId,
            CategoryName = wikiPage.Category?.Name,
            TemplateId = wikiPage.TemplateId,
            TemplateName = wikiPage.Template?.Name,
            CreatedByUserId = wikiPage.CreatedByUserId,
            CreatedByUserName = wikiPage.CreatedByUser?.UserName,
            LastUpdatedByUserId = wikiPage.LastUpdatedByUserId,
            LastUpdatedByUserName = wikiPage.LastUpdatedByUser?.UserName,
            IsPublished = wikiPage.IsPublished,
            ViewCount = wikiPage.ViewCount,
            CreatedDateTime = wikiPage.CreatedDateTime,
            UpdatedDateTime = wikiPage.UpdatedDateTime,
            Tags = wikiPage.Tags.Select(t => new WikiTagViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                Description = t.Description,
                UsageCount = t.UsageCount
            }).ToList(),
            VersionCount = wikiPage.Versions.Count
        };
    }

    public WikiPage MapToEntity(CreateWikiPageRequest request, string userId)
    {
        return new WikiPage
        {
            Title = request.Title,
            Slug = GenerateSlug(request.Title),
            Content = request.Content,
            Summary = request.Summary,
            CategoryId = request.CategoryId,
            TemplateId = request.TemplateId,
            CreatedByUserId = userId,
            IsPublished = request.IsPublished
        };
    }

    public void MapUpdateToEntity(UpdateWikiPageRequest request, WikiPage wikiPage, string userId)
    {
        wikiPage.Title = request.Title;
        wikiPage.Slug = GenerateSlug(request.Title);
        wikiPage.Content = request.Content;
        wikiPage.Summary = request.Summary;
        wikiPage.CategoryId = request.CategoryId;
        wikiPage.IsPublished = request.IsPublished;
        wikiPage.LastUpdatedByUserId = userId;
    }

    public WikiPageVersionViewModel MapVersionToViewModel(WikiPageVersion version)
    {
        return new WikiPageVersionViewModel
        {
            Id = version.Id,
            WikiPageId = version.WikiPageId,
            VersionNumber = version.VersionNumber,
            Title = version.Title,
            Content = version.Content,
            Summary = version.Summary,
            UpdatedByUserId = version.UpdatedByUserId,
            UpdatedByUserName = version.UpdatedByUser?.UserName,
            ChangeDescription = version.ChangeDescription,
            CreatedDateTime = version.CreatedDateTime
        };
    }

    public WikiPageVersion CreateVersion(WikiPage wikiPage, string userId, string? changeDescription)
    {
        return new WikiPageVersion
        {
            WikiPageId = wikiPage.Id,
            Title = wikiPage.Title,
            Content = wikiPage.Content,
            Summary = wikiPage.Summary,
            UpdatedByUserId = userId,
            ChangeDescription = changeDescription
        };
    }

    private string GenerateSlug(string title)
    {
        // Simple slug generation: lowercase, replace spaces and special chars with hyphens
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");

        // Remove any characters that aren't alphanumeric or hyphens
        slug = new string(slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

        // Remove consecutive hyphens
        while (slug.Contains("--"))
        {
            slug = slug.Replace("--", "-");
        }

        return slug.Trim('-');
    }
}
