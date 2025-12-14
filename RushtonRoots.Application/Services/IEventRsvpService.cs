using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IEventRsvpService
{
    Task<EventRsvpViewModel?> GetByIdAsync(int id);
    Task<EventRsvpViewModel?> GetByEventAndUserAsync(int eventId, string userId);
    Task<List<EventRsvpViewModel>> GetByEventIdAsync(int eventId);
    Task<List<EventRsvpViewModel>> GetByUserIdAsync(string userId);
    Task<EventRsvpViewModel> CreateRsvpAsync(CreateEventRsvpRequest request, string userId);
    Task<EventRsvpViewModel> UpdateRsvpAsync(int id, UpdateEventRsvpRequest request, string userId);
    Task DeleteRsvpAsync(int id, string userId);
}
