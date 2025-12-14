using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface ICommentMapper
{
    CommentViewModel MapToViewModel(Comment comment);
    Comment MapToEntity(CreateCommentRequest request, string userId);
    void UpdateEntity(Comment comment, UpdateCommentRequest request);
}
