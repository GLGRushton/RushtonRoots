using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IPhotoAlbumService
{
    Task<PhotoAlbumViewModel?> GetByIdAsync(int id);
    Task<List<PhotoAlbumViewModel>> GetAllAsync();
    Task<List<PhotoAlbumViewModel>> GetByUserIdAsync(string userId);
    Task<PhotoAlbumViewModel> CreateAlbumAsync(CreatePhotoAlbumRequest request, string userId);
    Task<PhotoAlbumViewModel> UpdateAlbumAsync(int id, CreatePhotoAlbumRequest request);
    Task DeleteAlbumAsync(int id);
}
