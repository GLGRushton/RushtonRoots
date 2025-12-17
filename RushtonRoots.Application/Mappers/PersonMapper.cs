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
            MiddleName = person.MiddleName,
            LastName = person.LastName,
            Suffix = person.Suffix,
            Gender = person.Gender,
            DateOfBirth = person.DateOfBirth,
            PlaceOfBirth = person.PlaceOfBirth,
            DateOfDeath = person.DateOfDeath,
            PlaceOfDeath = person.PlaceOfDeath,
            IsDeceased = person.IsDeceased,
            Biography = person.Biography,
            Occupation = person.Occupation,
            Education = person.Education,
            Notes = person.Notes,
            PhotoUrl = person.PhotoUrl,
            CreatedDateTime = person.CreatedDateTime,
            UpdatedDateTime = person.UpdatedDateTime
        };
    }

    public Person MapToEntity(CreatePersonRequest request)
    {
        return new Person
        {
            HouseholdId = request.HouseholdId.GetValueOrDefault(1), // Default to household 1 if not specified
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Suffix = request.Suffix,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            PlaceOfBirth = request.PlaceOfBirth,
            DateOfDeath = request.DateOfDeath,
            PlaceOfDeath = request.PlaceOfDeath,
            IsDeceased = request.IsDeceased,
            Biography = request.Biography,
            Occupation = request.Occupation,
            Education = request.Education,
            Notes = request.Notes,
            PhotoUrl = request.PhotoUrl
        };
    }

    public void MapToEntity(UpdatePersonRequest request, Person person)
    {
        if (request.HouseholdId.HasValue)
            person.HouseholdId = request.HouseholdId.Value;
        person.FirstName = request.FirstName;
        person.MiddleName = request.MiddleName;
        person.LastName = request.LastName;
        person.Suffix = request.Suffix;
        person.Gender = request.Gender;
        person.DateOfBirth = request.DateOfBirth;
        person.PlaceOfBirth = request.PlaceOfBirth;
        person.DateOfDeath = request.DateOfDeath;
        person.PlaceOfDeath = request.PlaceOfDeath;
        person.IsDeceased = request.IsDeceased;
        person.Biography = request.Biography;
        person.Occupation = request.Occupation;
        person.Education = request.Education;
        person.Notes = request.Notes;
        person.PhotoUrl = request.PhotoUrl;
    }
}
