using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IPhotoAlbumMapper
{
    PhotoAlbumViewModel MapToViewModel(PhotoAlbum album);
    PhotoAlbum MapToEntity(CreatePhotoAlbumRequest request, string userId);
}
