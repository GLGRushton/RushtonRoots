using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IPhotoAlbumRepository
{
    Task<PhotoAlbum?> GetByIdAsync(int id);
    Task<List<PhotoAlbum>> GetAllAsync();
    Task<List<PhotoAlbum>> GetByUserIdAsync(string userId);
    Task<PhotoAlbum> AddAsync(PhotoAlbum album);
    Task<PhotoAlbum> UpdateAsync(PhotoAlbum album);
    Task DeleteAsync(int id);
}
