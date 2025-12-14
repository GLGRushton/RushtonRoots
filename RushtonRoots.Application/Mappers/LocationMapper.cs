using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class LocationMapper : ILocationMapper
{
    public LocationViewModel MapToViewModel(Location location)
    {
        return new LocationViewModel
        {
            Id = location.Id,
            Name = location.Name,
            AddressLine1 = location.AddressLine1,
            AddressLine2 = location.AddressLine2,
            City = location.City,
            State = location.State,
            Country = location.Country,
            PostalCode = location.PostalCode,
            Latitude = location.Latitude,
            Longitude = location.Longitude
        };
    }

    public List<LocationViewModel> MapToViewModels(List<Location> locations)
    {
        return locations.Select(MapToViewModel).ToList();
    }

    public Location MapToEntity(CreateLocationRequest request)
    {
        return new Location
        {
            Name = request.Name,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };
    }
}
