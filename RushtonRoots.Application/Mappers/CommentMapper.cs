using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class CommentMapper : ICommentMapper
{
    public CommentViewModel MapToViewModel(Comment comment)
    {
        return new CommentViewModel
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            UserName = comment.User?.UserName,
            EntityType = comment.EntityType,
            EntityId = comment.EntityId,
            ParentCommentId = comment.ParentCommentId,
            IsEdited = comment.IsEdited,
            EditedAt = comment.EditedAt,
            CreatedDateTime = comment.CreatedDateTime,
            UpdatedDateTime = comment.UpdatedDateTime,
            Replies = comment.Replies?.Select(MapToViewModel).ToList() ?? new List<CommentViewModel>()
        };
    }

    public Comment MapToEntity(CreateCommentRequest request, string userId)
    {
        return new Comment
        {
            Content = request.Content,
            UserId = userId,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            ParentCommentId = request.ParentCommentId,
            IsEdited = false
        };
    }

    public void UpdateEntity(Comment comment, UpdateCommentRequest request)
    {
        comment.Content = request.Content;
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;
    }
}
