using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper implementation for Person entity.
/// </summary>
public class PersonMapper : IPersonMapper
{
    public PersonViewModel MapToViewModel(Person person)
    {
        return new PersonViewModel
        {
            Id = person.Id,
            HouseholdId = person.HouseholdId,
            HouseholdName = person.Household?.HouseholdName ?? string.Empty,
            FirstName = person.FirstName,
            LastName = person.LastName,
            DateOfBirth = person.DateOfBirth,
            DateOfDeath = person.DateOfDeath,
            IsDeceased = person.IsDeceased,
            PhotoUrl = person.PhotoUrl,
            CreatedDateTime = person.CreatedDateTime,
            UpdatedDateTime = person.UpdatedDateTime
        };
    }

    public Person MapToEntity(CreatePersonRequest request)
    {
        return new Person
        {
            HouseholdId = request.HouseholdId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            DateOfDeath = request.DateOfDeath,
            IsDeceased = request.IsDeceased,
            PhotoUrl = request.PhotoUrl
        };
    }

    public void MapToEntity(UpdatePersonRequest request, Person person)
    {
        person.HouseholdId = request.HouseholdId;
        person.FirstName = request.FirstName;
        person.LastName = request.LastName;
        person.DateOfBirth = request.DateOfBirth;
        person.DateOfDeath = request.DateOfDeath;
        person.IsDeceased = request.IsDeceased;
        person.PhotoUrl = request.PhotoUrl;
    }
}
