using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper interface for Household entity.
/// </summary>
public interface IHouseholdMapper
{
    HouseholdViewModel MapToViewModel(Household household, int memberCount);
    Household MapToEntity(CreateHouseholdRequest request);
    void MapToEntity(UpdateHouseholdRequest request, Household household);
}
