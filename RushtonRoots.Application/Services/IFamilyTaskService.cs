using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IFamilyTaskService
{
    Task<FamilyTaskViewModel?> GetByIdAsync(int id);
    Task<List<FamilyTaskViewModel>> GetAllAsync();
    Task<List<FamilyTaskViewModel>> GetByHouseholdIdAsync(int householdId);
    Task<List<FamilyTaskViewModel>> GetByAssignedUserIdAsync(string userId);
    Task<List<FamilyTaskViewModel>> GetByStatusAsync(string status);
    Task<List<FamilyTaskViewModel>> GetByEventIdAsync(int eventId);
    Task<FamilyTaskViewModel> CreateTaskAsync(CreateFamilyTaskRequest request, string createdByUserId);
    Task<FamilyTaskViewModel> UpdateTaskAsync(int id, UpdateFamilyTaskRequest request, string userId);
    Task DeleteTaskAsync(int id, string userId);
}
