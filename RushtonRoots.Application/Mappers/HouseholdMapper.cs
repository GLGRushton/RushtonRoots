using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper implementation for Household entity.
/// </summary>
public class HouseholdMapper : IHouseholdMapper
{
    public HouseholdViewModel MapToViewModel(Household household, int memberCount)
    {
        return new HouseholdViewModel
        {
            Id = household.Id,
            HouseholdName = household.HouseholdName,
            AnchorPersonId = household.AnchorPersonId,
            AnchorPersonName = household.AnchorPerson != null
                ? $"{household.AnchorPerson.FirstName} {household.AnchorPerson.LastName}"
                : string.Empty,
            MemberCount = memberCount,
            CreatedDateTime = household.CreatedDateTime,
            UpdatedDateTime = household.UpdatedDateTime
        };
    }

    public Household MapToEntity(CreateHouseholdRequest request)
    {
        return new Household
        {
            HouseholdName = request.HouseholdName,
            AnchorPersonId = request.AnchorPersonId
        };
    }

    public void MapToEntity(UpdateHouseholdRequest request, Household household)
    {
        household.HouseholdName = request.HouseholdName;
        household.AnchorPersonId = request.AnchorPersonId;
    }
}
