using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public interface IMediaMapper
{
    MediaViewModel MapToViewModel(Media media);
    MediaTimelineMarkerViewModel MapToViewModel(MediaTimelineMarker marker);
    Media MapToEntity(CreateMediaRequest request, string userId, string mediaUrl, string blobName, long fileSize, string contentType);
    void UpdateEntity(Media media, UpdateMediaRequest request);
    MediaTimelineMarker MapToEntity(CreateMediaTimelineMarkerRequest request, int mediaId);
}
