using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IDocumentMapper
{
    DocumentViewModel MapToViewModel(Document document);
    DocumentVersionViewModel MapToViewModel(DocumentVersion version);
    Document MapToEntity(CreateDocumentRequest request, string userId, string documentUrl, string blobName, long fileSize, string contentType);
    void UpdateEntity(Document document, UpdateDocumentRequest request);
}
