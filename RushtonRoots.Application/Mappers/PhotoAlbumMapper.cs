using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class PhotoAlbumMapper : IPhotoAlbumMapper
{
    public PhotoAlbumViewModel MapToViewModel(PhotoAlbum album)
    {
        return new PhotoAlbumViewModel
        {
            Id = album.Id,
            Name = album.Name,
            Description = album.Description,
            CreatedByUserId = album.CreatedByUserId,
            CreatedByUserName = album.CreatedBy?.UserName,
            AlbumDate = album.AlbumDate,
            CoverPhotoUrl = album.CoverPhotoUrl,
            IsPublic = album.IsPublic,
            DisplayOrder = album.DisplayOrder,
            PhotoCount = album.Photos?.Count ?? 0,
            CreatedDateTime = album.CreatedDateTime
        };
    }

    public PhotoAlbum MapToEntity(CreatePhotoAlbumRequest request, string userId)
    {
        return new PhotoAlbum
        {
            Name = request.Name,
            Description = request.Description,
            CreatedByUserId = userId,
            AlbumDate = request.AlbumDate,
            IsPublic = request.IsPublic,
            DisplayOrder = 0
        };
    }
}
