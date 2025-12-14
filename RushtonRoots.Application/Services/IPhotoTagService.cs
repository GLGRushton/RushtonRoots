using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IPhotoTagService
{
    Task<PhotoTagViewModel?> GetByIdAsync(int id);
    Task<List<PhotoTagViewModel>> GetByPhotoIdAsync(int photoId);
    Task<List<PhotoTagViewModel>> GetByPersonIdAsync(int personId);
    Task<PhotoTagViewModel> CreateTagAsync(CreatePhotoTagRequest request);
    Task DeleteTagAsync(int id);
}
