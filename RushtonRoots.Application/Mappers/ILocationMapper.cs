using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface ILocationMapper
{
    LocationViewModel MapToViewModel(Location location);
    List<LocationViewModel> MapToViewModels(List<Location> locations);
    Location MapToEntity(CreateLocationRequest request);
}
