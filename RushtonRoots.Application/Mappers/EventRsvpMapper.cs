using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class EventRsvpMapper : IEventRsvpMapper
{
    public EventRsvpViewModel MapToViewModel(EventRsvp eventRsvp)
    {
        return new EventRsvpViewModel
        {
            Id = eventRsvp.Id,
            FamilyEventId = eventRsvp.FamilyEventId,
            UserId = eventRsvp.UserId,
            UserName = eventRsvp.User?.UserName,
            Status = eventRsvp.Status,
            GuestCount = eventRsvp.GuestCount,
            Notes = eventRsvp.Notes,
            ResponseDateTime = eventRsvp.ResponseDateTime,
            CreatedDateTime = eventRsvp.CreatedDateTime,
            UpdatedDateTime = eventRsvp.UpdatedDateTime
        };
    }

    public EventRsvp MapToEntity(CreateEventRsvpRequest request, string userId)
    {
        return new EventRsvp
        {
            FamilyEventId = request.FamilyEventId,
            UserId = userId,
            Status = request.Status,
            GuestCount = request.GuestCount,
            Notes = request.Notes,
            ResponseDateTime = DateTime.UtcNow
        };
    }

    public void UpdateEntity(EventRsvp eventRsvp, UpdateEventRsvpRequest request)
    {
        eventRsvp.Status = request.Status;
        eventRsvp.GuestCount = request.GuestCount;
        eventRsvp.Notes = request.Notes;
        eventRsvp.ResponseDateTime = DateTime.UtcNow;
    }
}
