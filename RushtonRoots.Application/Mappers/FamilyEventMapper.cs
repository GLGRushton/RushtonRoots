using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class FamilyEventMapper : IFamilyEventMapper
{
    private readonly IEventRsvpMapper _rsvpMapper;

    public FamilyEventMapper(IEventRsvpMapper rsvpMapper)
    {
        _rsvpMapper = rsvpMapper;
    }

    public FamilyEventViewModel MapToViewModel(FamilyEvent familyEvent)
    {
        var rsvps = familyEvent.Rsvps?.Select(_rsvpMapper.MapToViewModel).ToList() ?? new List<EventRsvpViewModel>();
        
        return new FamilyEventViewModel
        {
            Id = familyEvent.Id,
            Title = familyEvent.Title,
            Description = familyEvent.Description,
            StartDateTime = familyEvent.StartDateTime,
            EndDateTime = familyEvent.EndDateTime,
            Location = familyEvent.Location,
            IsAllDay = familyEvent.IsAllDay,
            EventType = familyEvent.EventType,
            IsRecurring = familyEvent.IsRecurring,
            RecurrencePattern = familyEvent.RecurrencePattern,
            CreatedByUserId = familyEvent.CreatedByUserId,
            CreatedByUserName = familyEvent.CreatedByUser?.UserName,
            HouseholdId = familyEvent.HouseholdId,
            HouseholdName = familyEvent.Household?.HouseholdName,
            IsCancelled = familyEvent.IsCancelled,
            CreatedDateTime = familyEvent.CreatedDateTime,
            UpdatedDateTime = familyEvent.UpdatedDateTime,
            Rsvps = rsvps,
            AttendingCount = rsvps.Count(r => r.Status == "Attending"),
            NotAttendingCount = rsvps.Count(r => r.Status == "NotAttending"),
            MaybeCount = rsvps.Count(r => r.Status == "Maybe")
        };
    }

    public FamilyEvent MapToEntity(CreateFamilyEventRequest request, string createdByUserId)
    {
        return new FamilyEvent
        {
            Title = request.Title,
            Description = request.Description,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            Location = request.Location,
            IsAllDay = request.IsAllDay,
            EventType = request.EventType,
            IsRecurring = request.IsRecurring,
            RecurrencePattern = request.RecurrencePattern,
            CreatedByUserId = createdByUserId,
            HouseholdId = request.HouseholdId,
            IsCancelled = false
        };
    }

    public void UpdateEntity(FamilyEvent familyEvent, UpdateFamilyEventRequest request)
    {
        familyEvent.Title = request.Title;
        familyEvent.Description = request.Description;
        familyEvent.StartDateTime = request.StartDateTime;
        familyEvent.EndDateTime = request.EndDateTime;
        familyEvent.Location = request.Location;
        familyEvent.IsAllDay = request.IsAllDay;
        familyEvent.EventType = request.EventType;
        familyEvent.IsRecurring = request.IsRecurring;
        familyEvent.RecurrencePattern = request.RecurrencePattern;
        familyEvent.HouseholdId = request.HouseholdId;
        familyEvent.IsCancelled = request.IsCancelled;
    }
}
