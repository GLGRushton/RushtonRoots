using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IPhotoTagRepository
{
    Task<PhotoTag?> GetByIdAsync(int id);
    Task<List<PhotoTag>> GetByPhotoIdAsync(int photoId);
    Task<List<PhotoTag>> GetByPersonIdAsync(int personId);
    Task<PhotoTag> AddAsync(PhotoTag tag);
    Task DeleteAsync(int id);
}
