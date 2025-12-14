using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface ILifeEventMapper
{
    LifeEventViewModel MapToViewModel(LifeEvent lifeEvent);
    List<LifeEventViewModel> MapToViewModels(List<LifeEvent> lifeEvents);
    LifeEvent MapToEntity(CreateLifeEventRequest request);
    void MapToEntity(UpdateLifeEventRequest request, LifeEvent lifeEvent);
}
