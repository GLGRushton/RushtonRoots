using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IEventRsvpMapper
{
    EventRsvpViewModel MapToViewModel(EventRsvp eventRsvp);
    EventRsvp MapToEntity(CreateEventRsvpRequest request, string userId);
    void UpdateEntity(EventRsvp eventRsvp, UpdateEventRsvpRequest request);
}
