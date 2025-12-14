using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(int id);
    Task<List<Document>> GetAllAsync();
    Task<List<Document>> GetByUserIdAsync(string userId);
    Task<List<Document>> GetByCategoryAsync(string category);
    Task<List<Document>> GetByPersonIdAsync(int personId);
    Task<List<Document>> SearchAsync(SearchDocumentRequest request);
    Task<Document> AddAsync(Document document);
    Task<Document> UpdateAsync(Document document);
    Task DeleteAsync(int id);
    Task<DocumentVersion?> GetVersionByIdAsync(int versionId);
    Task<List<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId);
    Task<DocumentVersion> AddVersionAsync(DocumentVersion version);
    Task<DocumentPerson> AddDocumentPersonAsync(DocumentPerson documentPerson);
    Task RemoveDocumentPersonAsync(int documentId, int personId);
}
