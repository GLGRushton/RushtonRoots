using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IFamilyTaskMapper
{
    FamilyTaskViewModel MapToViewModel(FamilyTask familyTask);
    FamilyTask MapToEntity(CreateFamilyTaskRequest request, string createdByUserId);
    void UpdateEntity(FamilyTask familyTask, UpdateFamilyTaskRequest request);
}
