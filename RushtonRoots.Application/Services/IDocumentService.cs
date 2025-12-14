using Microsoft.AspNetCore.Http;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IDocumentService
{
    Task<DocumentViewModel?> GetByIdAsync(int id);
    Task<List<DocumentViewModel>> GetAllAsync();
    Task<List<DocumentViewModel>> GetByUserIdAsync(string userId);
    Task<List<DocumentViewModel>> GetByCategoryAsync(string category);
    Task<List<DocumentViewModel>> GetByPersonIdAsync(int personId);
    Task<List<DocumentViewModel>> SearchAsync(SearchDocumentRequest request);
    Task<DocumentViewModel> UploadDocumentAsync(CreateDocumentRequest request, IFormFile file, string userId);
    Task<DocumentViewModel> UpdateDocumentAsync(int id, UpdateDocumentRequest request);
    Task DeleteDocumentAsync(int id);
    Task<DocumentVersionViewModel> UploadNewVersionAsync(int documentId, IFormFile file, string userId, string? changeNotes);
    Task<List<DocumentVersionViewModel>> GetVersionsAsync(int documentId);
    Task<string> GetDocumentPreviewUrlAsync(int documentId);
}
