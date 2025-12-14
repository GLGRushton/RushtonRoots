using Microsoft.AspNetCore.Http;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using RushtonRoots.Infrastructure.Services;

namespace RushtonRoots.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;

    public DocumentService(
        IDocumentRepository documentRepository, 
        IDocumentMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }

    public async Task<DocumentViewModel?> GetByIdAsync(int id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        return document == null ? null : _mapper.MapToViewModel(document);
    }

    public async Task<List<DocumentViewModel>> GetAllAsync()
    {
        var documents = await _documentRepository.GetAllAsync();
        return documents.Select(d => _mapper.MapToViewModel(d)).ToList();
    }

    public async Task<List<DocumentViewModel>> GetByUserIdAsync(string userId)
    {
        var documents = await _documentRepository.GetByUserIdAsync(userId);
        return documents.Select(d => _mapper.MapToViewModel(d)).ToList();
    }

    public async Task<List<DocumentViewModel>> GetByCategoryAsync(string category)
    {
        var documents = await _documentRepository.GetByCategoryAsync(category);
        return documents.Select(d => _mapper.MapToViewModel(d)).ToList();
    }

    public async Task<List<DocumentViewModel>> GetByPersonIdAsync(int personId)
    {
        var documents = await _documentRepository.GetByPersonIdAsync(personId);
        return documents.Select(d => _mapper.MapToViewModel(d)).ToList();
    }

    public async Task<List<DocumentViewModel>> SearchAsync(SearchDocumentRequest request)
    {
        var documents = await _documentRepository.SearchAsync(request);
        return documents.Select(d => _mapper.MapToViewModel(d)).ToList();
    }

    public async Task<DocumentViewModel> UploadDocumentAsync(CreateDocumentRequest request, IFormFile file, string userId)
    {
        // Upload file to blob storage
        using var stream = file.OpenReadStream();
        var documentUrl = await _blobStorageService.UploadFileAsync(file.FileName, file.ContentType, stream);
        var blobName = ExtractBlobNameFromUrl(documentUrl);

        // Create document entity
        var document = _mapper.MapToEntity(request, userId, documentUrl, blobName, file.Length, file.ContentType);
        
        // Save document
        var savedDocument = await _documentRepository.AddAsync(document);

        // Create initial version (version 1)
        var version = new DocumentVersion
        {
            DocumentId = savedDocument.Id,
            DocumentUrl = documentUrl,
            BlobName = blobName,
            FileSize = file.Length,
            ContentType = file.ContentType,
            VersionNumber = 1,
            ChangeNotes = "Initial upload",
            UploadedByUserId = userId
        };
        await _documentRepository.AddVersionAsync(version);

        // Add person associations
        foreach (var personId in request.AssociatedPeople)
        {
            var documentPerson = new DocumentPerson
            {
                DocumentId = savedDocument.Id,
                PersonId = personId
            };
            await _documentRepository.AddDocumentPersonAsync(documentPerson);
        }

        // Reload document with all associations
        var reloadedDocument = await _documentRepository.GetByIdAsync(savedDocument.Id);
        return _mapper.MapToViewModel(reloadedDocument!);
    }

    public async Task<DocumentViewModel> UpdateDocumentAsync(int id, UpdateDocumentRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
        {
            throw new KeyNotFoundException($"Document with ID {id} not found");
        }

        // Update document metadata
        _mapper.UpdateEntity(document, request);
        await _documentRepository.UpdateAsync(document);

        // Update person associations
        // Remove existing associations
        var existingPeople = document.DocumentPeople.Select(dp => dp.PersonId).ToList();
        foreach (var personId in existingPeople)
        {
            if (!request.AssociatedPeople.Contains(personId))
            {
                await _documentRepository.RemoveDocumentPersonAsync(id, personId);
            }
        }

        // Add new associations
        foreach (var personId in request.AssociatedPeople)
        {
            if (!existingPeople.Contains(personId))
            {
                var documentPerson = new DocumentPerson
                {
                    DocumentId = id,
                    PersonId = personId
                };
                await _documentRepository.AddDocumentPersonAsync(documentPerson);
            }
        }

        // Reload document with all associations
        var reloadedDocument = await _documentRepository.GetByIdAsync(id);
        return _mapper.MapToViewModel(reloadedDocument!);
    }

    public async Task DeleteDocumentAsync(int id)
    {
        var document = await _documentRepository.GetByIdAsync(id);
        if (document == null)
        {
            throw new KeyNotFoundException($"Document with ID {id} not found");
        }

        // Delete file from blob storage
        if (!string.IsNullOrEmpty(document.BlobName))
        {
            await _blobStorageService.DeleteFileAsync(document.BlobName);
        }

        // Delete all versions from blob storage
        foreach (var version in document.Versions)
        {
            if (!string.IsNullOrEmpty(version.BlobName))
            {
                await _blobStorageService.DeleteFileAsync(version.BlobName);
            }
        }

        // Delete document (cascades to versions and associations)
        await _documentRepository.DeleteAsync(id);
    }

    public async Task<DocumentVersionViewModel> UploadNewVersionAsync(int documentId, IFormFile file, string userId, string? changeNotes)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        // Upload new version to blob storage
        using var stream = file.OpenReadStream();
        var documentUrl = await _blobStorageService.UploadFileAsync(file.FileName, file.ContentType, stream);
        var blobName = ExtractBlobNameFromUrl(documentUrl);

        // Get next version number - using database constraint to prevent duplicates
        // The unique index on (DocumentId, VersionNumber) will handle race conditions
        var versions = await _documentRepository.GetVersionsByDocumentIdAsync(documentId);
        var nextVersionNumber = versions.Any() ? versions.Max(v => v.VersionNumber) + 1 : 1;

        try
        {
            // Create new version
            var version = new DocumentVersion
            {
                DocumentId = documentId,
                DocumentUrl = documentUrl,
                BlobName = blobName,
                FileSize = file.Length,
                ContentType = file.ContentType,
                VersionNumber = nextVersionNumber,
                ChangeNotes = changeNotes ?? "Updated document",
                UploadedByUserId = userId
            };
            var savedVersion = await _documentRepository.AddVersionAsync(version);

            // Update main document to point to latest version
            document.DocumentUrl = documentUrl;
            document.BlobName = blobName;
            document.FileSize = file.Length;
            document.ContentType = file.ContentType;
            await _documentRepository.UpdateAsync(document);

            return _mapper.MapToViewModel(savedVersion);
        }
        catch (Exception)
        {
            // If version creation or document update fails, clean up the uploaded blob
            try
            {
                await _blobStorageService.DeleteFileAsync(blobName);
            }
            catch
            {
                // Log but don't throw - the original exception is more important
            }
            throw;
        }
    }

    public async Task<List<DocumentVersionViewModel>> GetVersionsAsync(int documentId)
    {
        var versions = await _documentRepository.GetVersionsByDocumentIdAsync(documentId);
        return versions.Select(v => _mapper.MapToViewModel(v)).ToList();
    }

    public async Task<string> GetDocumentPreviewUrlAsync(int documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null)
        {
            throw new KeyNotFoundException($"Document with ID {documentId} not found");
        }

        if (string.IsNullOrEmpty(document.BlobName))
        {
            throw new InvalidOperationException("Document has no associated blob");
        }

        // Generate a time-limited SAS URL for preview (valid for 1 hour)
        return await _blobStorageService.GetSasUrlAsync(document.BlobName, 60);
    }

    private string ExtractBlobNameFromUrl(string url)
    {
        try
        {
            // Extract the blob name from the full URL
            // Example: https://account.blob.core.windows.net/container/blobname -> blobname
            var uri = new Uri(url);
            var segments = uri.Segments;
            return segments.Length > 0 ? segments[^1] : string.Empty;
        }
        catch (UriFormatException)
        {
            // If URL is malformed, return the original string
            // This shouldn't happen with BlobStorageService, but handle gracefully
            return url;
        }
    }
}
