using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class LifeEventMapper : ILifeEventMapper
{
    public LifeEventViewModel MapToViewModel(LifeEvent lifeEvent)
    {
        return new LifeEventViewModel
        {
            Id = lifeEvent.Id,
            PersonId = lifeEvent.PersonId,
            EventType = lifeEvent.EventType,
            Title = lifeEvent.Title,
            Description = lifeEvent.Description,
            EventDate = lifeEvent.EventDate,
            LocationId = lifeEvent.LocationId,
            LocationName = lifeEvent.Location?.Name,
            SourceId = lifeEvent.SourceId,
            SourceTitle = lifeEvent.Source?.Title
        };
    }

    public List<LifeEventViewModel> MapToViewModels(List<LifeEvent> lifeEvents)
    {
        return lifeEvents.Select(MapToViewModel).ToList();
    }

    public LifeEvent MapToEntity(CreateLifeEventRequest request)
    {
        return new LifeEvent
        {
            PersonId = request.PersonId,
            EventType = request.EventType,
            Title = request.Title,
            Description = request.Description,
            EventDate = request.EventDate,
            LocationId = request.LocationId,
            SourceId = request.SourceId
        };
    }

    public void MapToEntity(UpdateLifeEventRequest request, LifeEvent lifeEvent)
    {
        lifeEvent.EventType = request.EventType;
        lifeEvent.Title = request.Title;
        lifeEvent.Description = request.Description;
        lifeEvent.EventDate = request.EventDate;
        lifeEvent.LocationId = request.LocationId;
        lifeEvent.SourceId = request.SourceId;
    }
}
