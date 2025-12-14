using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IEventRsvpRepository
{
    Task<EventRsvp?> GetByIdAsync(int id);
    Task<EventRsvp?> GetByEventAndUserAsync(int eventId, string userId);
    Task<IEnumerable<EventRsvp>> GetByEventIdAsync(int eventId);
    Task<IEnumerable<EventRsvp>> GetByUserIdAsync(string userId);
    Task<EventRsvp> AddAsync(EventRsvp eventRsvp);
    Task<EventRsvp> UpdateAsync(EventRsvp eventRsvp);
    Task DeleteAsync(int id);
}
