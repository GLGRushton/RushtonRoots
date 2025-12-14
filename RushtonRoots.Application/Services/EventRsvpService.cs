using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class EventRsvpService : IEventRsvpService
{
    private readonly IEventRsvpRepository _rsvpRepository;
    private readonly IEventRsvpMapper _mapper;
    private readonly INotificationService _notificationService;

    public EventRsvpService(
        IEventRsvpRepository rsvpRepository,
        IEventRsvpMapper mapper,
        INotificationService notificationService)
    {
        _rsvpRepository = rsvpRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<EventRsvpViewModel?> GetByIdAsync(int id)
    {
        var rsvp = await _rsvpRepository.GetByIdAsync(id);
        return rsvp == null ? null : _mapper.MapToViewModel(rsvp);
    }

    public async Task<EventRsvpViewModel?> GetByEventAndUserAsync(int eventId, string userId)
    {
        var rsvp = await _rsvpRepository.GetByEventAndUserAsync(eventId, userId);
        return rsvp == null ? null : _mapper.MapToViewModel(rsvp);
    }

    public async Task<List<EventRsvpViewModel>> GetByEventIdAsync(int eventId)
    {
        var rsvps = await _rsvpRepository.GetByEventIdAsync(eventId);
        return rsvps.Select(r => _mapper.MapToViewModel(r)).ToList();
    }

    public async Task<List<EventRsvpViewModel>> GetByUserIdAsync(string userId)
    {
        var rsvps = await _rsvpRepository.GetByUserIdAsync(userId);
        return rsvps.Select(r => _mapper.MapToViewModel(r)).ToList();
    }

    public async Task<EventRsvpViewModel> CreateRsvpAsync(CreateEventRsvpRequest request, string userId)
    {
        // Check if RSVP already exists
        var existingRsvp = await _rsvpRepository.GetByEventAndUserAsync(request.FamilyEventId, userId);
        if (existingRsvp != null)
        {
            throw new InvalidOperationException("You have already RSVPed to this event");
        }

        var rsvp = _mapper.MapToEntity(request, userId);
        var savedRsvp = await _rsvpRepository.AddAsync(rsvp);
        
        // Create notification for RSVP
        await _notificationService.CreateNotificationAsync(
            userId,
            "Event",
            "RSVP Confirmed",
            $"Your RSVP has been recorded: {request.Status}",
            $"/events/{request.FamilyEventId}",
            request.FamilyEventId,
            "EventRsvp");
        
        return _mapper.MapToViewModel(savedRsvp);
    }

    public async Task<EventRsvpViewModel> UpdateRsvpAsync(int id, UpdateEventRsvpRequest request, string userId)
    {
        var rsvp = await _rsvpRepository.GetByIdAsync(id);
        if (rsvp == null)
        {
            throw new InvalidOperationException("RSVP not found");
        }

        if (rsvp.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update your own RSVP");
        }

        _mapper.UpdateEntity(rsvp, request);
        var updatedRsvp = await _rsvpRepository.UpdateAsync(rsvp);
        
        return _mapper.MapToViewModel(updatedRsvp);
    }

    public async Task DeleteRsvpAsync(int id, string userId)
    {
        var rsvp = await _rsvpRepository.GetByIdAsync(id);
        if (rsvp == null)
        {
            throw new InvalidOperationException("RSVP not found");
        }

        if (rsvp.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own RSVP");
        }

        await _rsvpRepository.DeleteAsync(id);
    }
}
