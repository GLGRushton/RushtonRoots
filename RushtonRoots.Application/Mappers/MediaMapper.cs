using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class MediaMapper : IMediaMapper
{
    public MediaViewModel MapToViewModel(Media media)
    {
        return new MediaViewModel
        {
            Id = media.Id,
            Title = media.Title,
            Description = media.Description,
            MediaUrl = media.MediaUrl,
            ThumbnailUrl = media.ThumbnailUrl,
            MediaType = media.MediaType,
            FileSize = media.FileSize,
            ContentType = media.ContentType,
            DurationSeconds = media.DurationSeconds,
            MediaDate = media.MediaDate,
            Transcription = media.Transcription,
            UploadedByUserId = media.UploadedByUserId,
            IsPublic = media.IsPublic,
            DisplayOrder = media.DisplayOrder,
            CreatedDateTime = media.CreatedDateTime,
            UpdatedDateTime = media.UpdatedDateTime,
            TimelineMarkers = media.TimelineMarkers?.Select(MapToViewModel).ToList() ?? new List<MediaTimelineMarkerViewModel>(),
            AssociatedPeopleIds = media.MediaPeople?.Select(mp => mp.PersonId).ToList() ?? new List<int>()
        };
    }

    public MediaTimelineMarkerViewModel MapToViewModel(MediaTimelineMarker marker)
    {
        return new MediaTimelineMarkerViewModel
        {
            Id = marker.Id,
            MediaId = marker.MediaId,
            TimeSeconds = marker.TimeSeconds,
            Label = marker.Label,
            Description = marker.Description,
            ThumbnailUrl = marker.ThumbnailUrl,
            CreatedDateTime = marker.CreatedDateTime,
            UpdatedDateTime = marker.UpdatedDateTime
        };
    }

    public Media MapToEntity(CreateMediaRequest request, string userId, string mediaUrl, string blobName, long fileSize, string contentType)
    {
        return new Media
        {
            Title = request.Title,
            Description = request.Description,
            MediaType = request.MediaType,
            MediaUrl = mediaUrl,
            BlobName = blobName,
            FileSize = fileSize,
            ContentType = contentType,
            MediaDate = request.MediaDate,
            UploadedByUserId = userId,
            IsPublic = request.IsPublic,
            DisplayOrder = request.DisplayOrder
        };
    }

    public void UpdateEntity(Media media, UpdateMediaRequest request)
    {
        media.Title = request.Title;
        media.Description = request.Description;
        media.MediaDate = request.MediaDate;
        media.Transcription = request.Transcription;
        media.IsPublic = request.IsPublic;
        media.DisplayOrder = request.DisplayOrder;
    }

    public MediaTimelineMarker MapToEntity(CreateMediaTimelineMarkerRequest request, int mediaId)
    {
        return new MediaTimelineMarker
        {
            MediaId = mediaId,
            TimeSeconds = request.TimeSeconds,
            Label = request.Label,
            Description = request.Description
        };
    }
}
