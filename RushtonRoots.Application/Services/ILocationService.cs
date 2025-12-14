using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface ILocationService
{
    Task<LocationViewModel?> GetByIdAsync(int id);
    Task<List<LocationViewModel>> GetAllAsync();
    Task<List<LocationViewModel>> SearchAsync(string searchTerm);
    Task<LocationViewModel> CreateAsync(CreateLocationRequest request);
    Task DeleteAsync(int id);
}
