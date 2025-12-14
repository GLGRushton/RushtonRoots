using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface ILifeEventService
{
    Task<LifeEventViewModel?> GetByIdAsync(int id);
    Task<List<LifeEventViewModel>> GetByPersonIdAsync(int personId);
    Task<PersonTimelineViewModel> GetPersonTimelineAsync(int personId);
    Task<LifeEventViewModel> CreateAsync(CreateLifeEventRequest request);
    Task<LifeEventViewModel> UpdateAsync(UpdateLifeEventRequest request);
    Task DeleteAsync(int id);
}
