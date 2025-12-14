using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IFamilyEventService
{
    Task<FamilyEventViewModel?> GetByIdAsync(int id);
    Task<List<FamilyEventViewModel>> GetAllAsync();
    Task<List<FamilyEventViewModel>> GetByHouseholdIdAsync(int householdId);
    Task<List<FamilyEventViewModel>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<FamilyEventViewModel>> GetUpcomingEventsAsync(int count = 10);
    Task<FamilyEventViewModel> CreateEventAsync(CreateFamilyEventRequest request, string createdByUserId);
    Task<FamilyEventViewModel> UpdateEventAsync(int id, UpdateFamilyEventRequest request, string userId);
    Task DeleteEventAsync(int id, string userId);
}
