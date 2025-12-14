using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class PhotoTagMapper : IPhotoTagMapper
{
    public PhotoTagViewModel MapToViewModel(PhotoTag tag)
    {
        return new PhotoTagViewModel
        {
            Id = tag.Id,
            PersonPhotoId = tag.PersonPhotoId,
            PersonId = tag.PersonId,
            PersonName = tag.Person != null 
                ? $"{tag.Person.FirstName} {tag.Person.LastName}".Trim() 
                : null,
            Notes = tag.Notes,
            XPosition = tag.XPosition,
            YPosition = tag.YPosition
        };
    }

    public PhotoTag MapToEntity(CreatePhotoTagRequest request)
    {
        return new PhotoTag
        {
            PersonPhotoId = request.PersonPhotoId,
            PersonId = request.PersonId,
            Notes = request.Notes,
            XPosition = request.XPosition,
            YPosition = request.YPosition
        };
    }
}
