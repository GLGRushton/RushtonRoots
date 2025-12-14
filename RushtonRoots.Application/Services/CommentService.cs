using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentMapper _mapper;

    public CommentService(
        ICommentRepository commentRepository,
        ICommentMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<CommentViewModel?> GetByIdAsync(int id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return comment == null ? null : _mapper.MapToViewModel(comment);
    }

    public async Task<List<CommentViewModel>> GetByEntityAsync(string entityType, int entityId)
    {
        var comments = await _commentRepository.GetByEntityAsync(entityType, entityId);
        return comments.Select(c => _mapper.MapToViewModel(c)).ToList();
    }

    public async Task<List<CommentViewModel>> GetByUserIdAsync(string userId)
    {
        var comments = await _commentRepository.GetByUserIdAsync(userId);
        return comments.Select(c => _mapper.MapToViewModel(c)).ToList();
    }

    public async Task<CommentViewModel> CreateCommentAsync(CreateCommentRequest request, string userId)
    {
        var comment = _mapper.MapToEntity(request, userId);
        var savedComment = await _commentRepository.AddAsync(comment);
        
        return _mapper.MapToViewModel(savedComment);
    }

    public async Task<CommentViewModel> UpdateCommentAsync(int id, UpdateCommentRequest request, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new InvalidOperationException("Comment not found");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update your own comments");
        }

        _mapper.UpdateEntity(comment, request);
        var updatedComment = await _commentRepository.UpdateAsync(comment);
        
        return _mapper.MapToViewModel(updatedComment);
    }

    public async Task DeleteCommentAsync(int id, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        if (comment == null)
        {
            throw new InvalidOperationException("Comment not found");
        }

        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own comments");
        }

        await _commentRepository.DeleteAsync(id);
    }
}
