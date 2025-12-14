using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class LifeEventService : ILifeEventService
{
    private readonly ILifeEventRepository _lifeEventRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ILifeEventMapper _lifeEventMapper;

    public LifeEventService(
        ILifeEventRepository lifeEventRepository,
        IPersonRepository personRepository,
        ILifeEventMapper lifeEventMapper)
    {
        _lifeEventRepository = lifeEventRepository;
        _personRepository = personRepository;
        _lifeEventMapper = lifeEventMapper;
    }

    public async Task<LifeEventViewModel?> GetByIdAsync(int id)
    {
        var lifeEvent = await _lifeEventRepository.GetByIdAsync(id);
        return lifeEvent == null ? null : _lifeEventMapper.MapToViewModel(lifeEvent);
    }

    public async Task<List<LifeEventViewModel>> GetByPersonIdAsync(int personId)
    {
        var lifeEvents = await _lifeEventRepository.GetByPersonIdAsync(personId);
        return _lifeEventMapper.MapToViewModels(lifeEvents);
    }

    public async Task<PersonTimelineViewModel> GetPersonTimelineAsync(int personId)
    {
        var person = await _personRepository.GetByIdAsync(personId);
        if (person == null)
        {
            throw new ArgumentException($"Person with ID {personId} not found.");
        }

        var lifeEvents = await _lifeEventRepository.GetByPersonIdAsync(personId);
        
        var timeline = new PersonTimelineViewModel
        {
            PersonId = personId,
            PersonName = $"{person.FirstName} {person.LastName}",
            Events = new List<TimelineEventViewModel>()
        };

        // Add birth event if date exists
        if (person.DateOfBirth.HasValue)
        {
            timeline.Events.Add(new TimelineEventViewModel
            {
                EventType = "Birth",
                Title = "Born",
                EventDate = person.DateOfBirth
            });
        }

        // Add life events
        foreach (var lifeEvent in lifeEvents)
        {
            timeline.Events.Add(new TimelineEventViewModel
            {
                Id = lifeEvent.Id,
                EventType = lifeEvent.EventType,
                Title = lifeEvent.Title,
                Description = lifeEvent.Description,
                EventDate = lifeEvent.EventDate,
                Location = lifeEvent.Location?.Name,
                Source = lifeEvent.Source?.Title
            });
        }

        // Add death event if date exists
        if (person.DateOfDeath.HasValue)
        {
            timeline.Events.Add(new TimelineEventViewModel
            {
                EventType = "Death",
                Title = "Died",
                EventDate = person.DateOfDeath
            });
        }

        // Sort events by date
        timeline.Events = timeline.Events
            .OrderBy(e => e.EventDate ?? DateTime.MaxValue)
            .ToList();

        return timeline;
    }

    public async Task<LifeEventViewModel> CreateAsync(CreateLifeEventRequest request)
    {
        var lifeEvent = _lifeEventMapper.MapToEntity(request);
        var created = await _lifeEventRepository.AddAsync(lifeEvent);
        
        // Reload with related data
        var reloaded = await _lifeEventRepository.GetByIdAsync(created.Id);
        return _lifeEventMapper.MapToViewModel(reloaded!);
    }

    public async Task<LifeEventViewModel> UpdateAsync(UpdateLifeEventRequest request)
    {
        var lifeEvent = await _lifeEventRepository.GetByIdAsync(request.Id);
        if (lifeEvent == null)
        {
            throw new ArgumentException($"Life event with ID {request.Id} not found.");
        }

        _lifeEventMapper.MapToEntity(request, lifeEvent);
        var updated = await _lifeEventRepository.UpdateAsync(lifeEvent);
        
        // Reload with related data
        var reloaded = await _lifeEventRepository.GetByIdAsync(updated.Id);
        return _lifeEventMapper.MapToViewModel(reloaded!);
    }

    public async Task DeleteAsync(int id)
    {
        await _lifeEventRepository.DeleteAsync(id);
    }
}
