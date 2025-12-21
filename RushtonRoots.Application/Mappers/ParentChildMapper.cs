using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper implementation for ParentChild entity.
/// </summary>
public class ParentChildMapper : IParentChildMapper
{
    public ParentChildViewModel MapToViewModel(ParentChild parentChild)
    {
        // Calculate child's age if birth date is available
        int? childAge = null;
        if (parentChild.ChildPerson?.DateOfBirth.HasValue == true)
        {
            var today = DateTime.Today;
            var birthDate = parentChild.ChildPerson.DateOfBirth.Value;
            
            // Use death date if child is deceased, otherwise use today
            var comparisonDate = parentChild.ChildPerson.IsDeceased && parentChild.ChildPerson.DateOfDeath.HasValue
                ? parentChild.ChildPerson.DateOfDeath.Value
                : today;
            
            var age = comparisonDate.Year - birthDate.Year;
            // Adjust age if birthday hasn't occurred yet this year
            if (birthDate.AddYears(age) > comparisonDate) age--;
            childAge = age >= 0 ? age : null;
        }

        return new ParentChildViewModel
        {
            Id = parentChild.Id,
            ParentPersonId = parentChild.ParentPersonId,
            ChildPersonId = parentChild.ChildPersonId,
            ParentName = parentChild.ParentPerson != null 
                ? $"{parentChild.ParentPerson.FirstName} {parentChild.ParentPerson.LastName}" 
                : "Unknown",
            ChildName = parentChild.ChildPerson != null 
                ? $"{parentChild.ChildPerson.FirstName} {parentChild.ChildPerson.LastName}" 
                : "Unknown",
            ParentPhotoUrl = parentChild.ParentPerson?.PhotoUrl,
            ChildPhotoUrl = parentChild.ChildPerson?.PhotoUrl,
            ParentBirthDate = parentChild.ParentPerson?.DateOfBirth,
            ParentDeathDate = parentChild.ParentPerson?.DateOfDeath,
            ChildBirthDate = parentChild.ChildPerson?.DateOfBirth,
            ChildDeathDate = parentChild.ChildPerson?.DateOfDeath,
            ChildAge = childAge,
            RelationshipType = parentChild.RelationshipType,
            Notes = parentChild.Notes,
            ConfidenceScore = parentChild.ConfidenceScore,
            // TODO: Implement verification logic when verification feature is added
            // For now, all relationships are marked as verified by default
            IsVerified = true,
            CreatedDateTime = parentChild.CreatedDateTime,
            UpdatedDateTime = parentChild.UpdatedDateTime
        };
    }

    public ParentChild MapToEntity(CreateParentChildRequest request)
    {
        return new ParentChild
        {
            ParentPersonId = request.ParentPersonId,
            ChildPersonId = request.ChildPersonId,
            RelationshipType = request.RelationshipType
        };
    }

    public void UpdateEntity(ParentChild entity, UpdateParentChildRequest request)
    {
        entity.ParentPersonId = request.ParentPersonId;
        entity.ChildPersonId = request.ChildPersonId;
        entity.RelationshipType = request.RelationshipType;
    }
}
