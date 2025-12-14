using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface ICommentService
{
    Task<CommentViewModel?> GetByIdAsync(int id);
    Task<List<CommentViewModel>> GetByEntityAsync(string entityType, int entityId);
    Task<List<CommentViewModel>> GetByUserIdAsync(string userId);
    Task<CommentViewModel> CreateCommentAsync(CreateCommentRequest request, string userId);
    Task<CommentViewModel> UpdateCommentAsync(int id, UpdateCommentRequest request, string userId);
    Task DeleteCommentAsync(int id, string userId);
}
