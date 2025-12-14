using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IMessageMapper
{
    MessageViewModel MapToViewModel(Message message);
    Message MapToEntity(CreateMessageRequest request, string senderUserId);
    void UpdateEntity(Message message, UpdateMessageRequest request);
}
