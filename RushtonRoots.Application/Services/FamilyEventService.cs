using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class FamilyEventService : IFamilyEventService
{
    private readonly IFamilyEventRepository _eventRepository;
    private readonly IFamilyEventMapper _mapper;
    private readonly INotificationService _notificationService;

    public FamilyEventService(
        IFamilyEventRepository eventRepository,
        IFamilyEventMapper mapper,
        INotificationService notificationService)
    {
        _eventRepository = eventRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<FamilyEventViewModel?> GetByIdAsync(int id)
    {
        var familyEvent = await _eventRepository.GetByIdAsync(id);
        return familyEvent == null ? null : _mapper.MapToViewModel(familyEvent);
    }

    public async Task<List<FamilyEventViewModel>> GetAllAsync()
    {
        var events = await _eventRepository.GetAllAsync();
        return events.Select(e => _mapper.MapToViewModel(e)).ToList();
    }

    public async Task<List<FamilyEventViewModel>> GetByHouseholdIdAsync(int householdId)
    {
        var events = await _eventRepository.GetByHouseholdIdAsync(householdId);
        return events.Select(e => _mapper.MapToViewModel(e)).ToList();
    }

    public async Task<List<FamilyEventViewModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var events = await _eventRepository.GetByDateRangeAsync(startDate, endDate);
        return events.Select(e => _mapper.MapToViewModel(e)).ToList();
    }

    public async Task<List<FamilyEventViewModel>> GetUpcomingEventsAsync(int count = 10)
    {
        var events = await _eventRepository.GetUpcomingEventsAsync(count);
        return events.Select(e => _mapper.MapToViewModel(e)).ToList();
    }

    public async Task<FamilyEventViewModel> CreateEventAsync(CreateFamilyEventRequest request, string createdByUserId)
    {
        var familyEvent = _mapper.MapToEntity(request, createdByUserId);
        var savedEvent = await _eventRepository.AddAsync(familyEvent);
        
        // Create notification for event creation (could notify household members)
        await _notificationService.CreateNotificationAsync(
            createdByUserId,
            "Event",
            "Event Created",
            $"You created a new event: {savedEvent.Title}",
            $"/events/{savedEvent.Id}",
            savedEvent.Id,
            "FamilyEvent");
        
        return _mapper.MapToViewModel(savedEvent);
    }

    public async Task<FamilyEventViewModel> UpdateEventAsync(int id, UpdateFamilyEventRequest request, string userId)
    {
        var familyEvent = await _eventRepository.GetByIdAsync(id);
        if (familyEvent == null)
        {
            throw new InvalidOperationException("Event not found");
        }

        if (familyEvent.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update events you created");
        }

        _mapper.UpdateEntity(familyEvent, request);
        var updatedEvent = await _eventRepository.UpdateAsync(familyEvent);
        
        return _mapper.MapToViewModel(updatedEvent);
    }

    public async Task DeleteEventAsync(int id, string userId)
    {
        var familyEvent = await _eventRepository.GetByIdAsync(id);
        if (familyEvent == null)
        {
            throw new InvalidOperationException("Event not found");
        }

        if (familyEvent.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete events you created");
        }

        await _eventRepository.DeleteAsync(id);
    }
}
