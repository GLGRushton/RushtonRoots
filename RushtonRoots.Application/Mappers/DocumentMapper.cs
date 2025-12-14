using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class DocumentMapper : IDocumentMapper
{
    public DocumentViewModel MapToViewModel(Document document)
    {
        // Get current version (highest version number)
        var currentVersion = document.Versions?.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
        
        return new DocumentViewModel
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            DocumentUrl = document.DocumentUrl,
            ThumbnailUrl = document.ThumbnailUrl,
            Category = document.Category,
            FileSize = document.FileSize,
            ContentType = document.ContentType,
            DocumentDate = document.DocumentDate,
            UploadedByUserId = document.UploadedByUserId,
            UploadedByUserName = document.UploadedBy?.UserName,
            IsPublic = document.IsPublic,
            DisplayOrder = document.DisplayOrder,
            CreatedDateTime = document.CreatedDateTime,
            UpdatedDateTime = document.UpdatedDateTime,
            VersionCount = document.Versions?.Count ?? 0,
            CurrentVersion = currentVersion?.VersionNumber ?? 1,
            AssociatedPeople = document.DocumentPeople?.Select(dp => dp.PersonId).ToList() ?? new List<int>()
        };
    }

    public DocumentVersionViewModel MapToViewModel(DocumentVersion version)
    {
        return new DocumentVersionViewModel
        {
            Id = version.Id,
            DocumentId = version.DocumentId,
            DocumentUrl = version.DocumentUrl,
            FileSize = version.FileSize,
            ContentType = version.ContentType,
            VersionNumber = version.VersionNumber,
            ChangeNotes = version.ChangeNotes,
            UploadedByUserId = version.UploadedByUserId,
            UploadedByUserName = version.UploadedBy?.UserName,
            CreatedDateTime = version.CreatedDateTime
        };
    }

    public Document MapToEntity(CreateDocumentRequest request, string userId, string documentUrl, string blobName, long fileSize, string contentType)
    {
        return new Document
        {
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            DocumentDate = request.DocumentDate,
            DocumentUrl = documentUrl,
            BlobName = blobName,
            FileSize = fileSize,
            ContentType = contentType,
            UploadedByUserId = userId,
            IsPublic = request.IsPublic,
            DisplayOrder = 0
        };
    }

    public void UpdateEntity(Document document, UpdateDocumentRequest request)
    {
        document.Title = request.Title;
        document.Description = request.Description;
        document.Category = request.Category;
        document.DocumentDate = request.DocumentDate;
        document.IsPublic = request.IsPublic;
    }
}
