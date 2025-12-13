using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

/// <summary>
/// Mapper interface for Person entity.
/// </summary>
public interface IPersonMapper
{
    PersonViewModel MapToViewModel(Person person);
    Person MapToEntity(CreatePersonRequest request);
    void MapToEntity(UpdatePersonRequest request, Person person);
}
