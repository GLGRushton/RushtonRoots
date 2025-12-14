using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IFamilyEventMapper
{
    FamilyEventViewModel MapToViewModel(FamilyEvent familyEvent);
    FamilyEvent MapToEntity(CreateFamilyEventRequest request, string createdByUserId);
    void UpdateEntity(FamilyEvent familyEvent, UpdateFamilyEventRequest request);
}
