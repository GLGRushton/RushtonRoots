using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILocationMapper _locationMapper;

    public LocationService(
        ILocationRepository locationRepository,
        ILocationMapper locationMapper)
    {
        _locationRepository = locationRepository;
        _locationMapper = locationMapper;
    }

    public async Task<LocationViewModel?> GetByIdAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        return location == null ? null : _locationMapper.MapToViewModel(location);
    }

    public async Task<List<LocationViewModel>> GetAllAsync()
    {
        var locations = await _locationRepository.GetAllAsync();
        return _locationMapper.MapToViewModels(locations);
    }

    public async Task<List<LocationViewModel>> SearchAsync(string searchTerm)
    {
        var locations = await _locationRepository.SearchAsync(searchTerm);
        return _locationMapper.MapToViewModels(locations);
    }

    public async Task<LocationViewModel> CreateAsync(CreateLocationRequest request)
    {
        var location = _locationMapper.MapToEntity(request);
        var created = await _locationRepository.AddAsync(location);
        return _locationMapper.MapToViewModel(created);
    }

    public async Task DeleteAsync(int id)
    {
        await _locationRepository.DeleteAsync(id);
    }
}
