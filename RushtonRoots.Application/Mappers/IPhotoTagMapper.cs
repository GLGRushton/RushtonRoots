using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IPhotoTagMapper
{
    PhotoTagViewModel MapToViewModel(PhotoTag tag);
    PhotoTag MapToEntity(CreatePhotoTagRequest request);
}
