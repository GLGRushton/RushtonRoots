using Microsoft.AspNetCore.Http;
using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using RushtonRoots.Infrastructure.Services;

namespace RushtonRoots.Application.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;

    public MediaService(
        IMediaRepository mediaRepository,
        IMediaMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _mediaRepository = mediaRepository;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }

    public async Task<MediaViewModel?> GetByIdAsync(int id)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        return media == null ? null : _mapper.MapToViewModel(media);
    }

    public async Task<List<MediaViewModel>> GetAllAsync()
    {
        var mediaList = await _mediaRepository.GetAllAsync();
        return mediaList.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task<List<MediaViewModel>> GetByUserIdAsync(string userId)
    {
        var mediaList = await _mediaRepository.GetByUserIdAsync(userId);
        return mediaList.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task<List<MediaViewModel>> GetByPersonIdAsync(int personId)
    {
        var mediaList = await _mediaRepository.GetByPersonIdAsync(personId);
        return mediaList.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task<List<MediaViewModel>> SearchAsync(SearchMediaRequest request)
    {
        var mediaList = await _mediaRepository.SearchAsync(request);
        return mediaList.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task<MediaViewModel> UploadMediaAsync(CreateMediaRequest request, IFormFile file, string userId)
    {
        // Upload file to blob storage
        using var stream = file.OpenReadStream();
        var mediaUrl = await _blobStorageService.UploadFileAsync(file.FileName, file.ContentType, stream);
        var blobName = ExtractBlobNameFromUrl(mediaUrl);

        try
        {
            // Create media entity
            var media = _mapper.MapToEntity(request, userId, mediaUrl, blobName, file.Length, file.ContentType);
            
            // Save media
            var savedMedia = await _mediaRepository.AddAsync(media);

            // Add person associations
            foreach (var personId in request.AssociatedPeople)
            {
                var mediaPerson = new MediaPerson
                {
                    MediaId = savedMedia.Id,
                    PersonId = personId
                };
                await _mediaRepository.AddMediaPersonAsync(mediaPerson);
            }

            // Reload media with all associations
            var reloadedMedia = await _mediaRepository.GetByIdAsync(savedMedia.Id);
            return _mapper.MapToViewModel(reloadedMedia!);
        }
        catch (Exception)
        {
            // If database operations fail, clean up the uploaded blob to prevent orphaned files
            try
            {
                await _blobStorageService.DeleteFileAsync(blobName);
            }
            catch
            {
                // Log but don't throw - the original exception is more important
                // In production, this should be logged for cleanup purposes
            }
            throw;
        }
    }

    public async Task<MediaViewModel> UpdateMediaAsync(int id, UpdateMediaRequest request)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        if (media == null)
        {
            throw new KeyNotFoundException($"Media with ID {id} not found");
        }

        // Update media metadata
        _mapper.UpdateEntity(media, request);
        await _mediaRepository.UpdateAsync(media);

        // Update person associations
        // Remove existing associations
        var existingPeople = media.MediaPeople.Select(mp => mp.PersonId).ToList();
        foreach (var personId in existingPeople)
        {
            if (!request.AssociatedPeople.Contains(personId))
            {
                await _mediaRepository.RemoveMediaPersonAsync(id, personId);
            }
        }

        // Add new associations
        foreach (var personId in request.AssociatedPeople)
        {
            if (!existingPeople.Contains(personId))
            {
                var mediaPerson = new MediaPerson
                {
                    MediaId = id,
                    PersonId = personId
                };
                await _mediaRepository.AddMediaPersonAsync(mediaPerson);
            }
        }

        // Reload media with all associations
        var reloadedMedia = await _mediaRepository.GetByIdAsync(id);
        return _mapper.MapToViewModel(reloadedMedia!);
    }

    public async Task DeleteMediaAsync(int id)
    {
        var media = await _mediaRepository.GetByIdAsync(id);
        if (media == null)
        {
            throw new KeyNotFoundException($"Media with ID {id} not found");
        }

        // Delete file from blob storage
        if (!string.IsNullOrEmpty(media.BlobName))
        {
            await _blobStorageService.DeleteFileAsync(media.BlobName);
        }

        // Delete media (cascades to timeline markers and associations)
        await _mediaRepository.DeleteAsync(id);
    }

    public async Task<string> GetMediaStreamUrlAsync(int mediaId)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId);
        if (media == null)
        {
            throw new KeyNotFoundException($"Media with ID {mediaId} not found");
        }

        if (string.IsNullOrEmpty(media.BlobName))
        {
            throw new InvalidOperationException("Media has no associated blob");
        }

        // Generate a time-limited SAS URL for streaming (valid for 4 hours for longer videos)
        return await _blobStorageService.GetSasUrlAsync(media.BlobName, 240);
    }

    public async Task<MediaTimelineMarkerViewModel> AddTimelineMarkerAsync(int mediaId, CreateMediaTimelineMarkerRequest request)
    {
        var media = await _mediaRepository.GetByIdAsync(mediaId);
        if (media == null)
        {
            throw new KeyNotFoundException($"Media with ID {mediaId} not found");
        }

        var marker = _mapper.MapToEntity(request, mediaId);
        var savedMarker = await _mediaRepository.AddMarkerAsync(marker);
        
        return _mapper.MapToViewModel(savedMarker);
    }

    public async Task<List<MediaTimelineMarkerViewModel>> GetTimelineMarkersAsync(int mediaId)
    {
        var markers = await _mediaRepository.GetMarkersByMediaIdAsync(mediaId);
        return markers.Select(m => _mapper.MapToViewModel(m)).ToList();
    }

    public async Task DeleteTimelineMarkerAsync(int markerId)
    {
        var marker = await _mediaRepository.GetMarkerByIdAsync(markerId);
        if (marker == null)
        {
            throw new KeyNotFoundException($"Timeline marker with ID {markerId} not found");
        }
        
        await _mediaRepository.DeleteMarkerAsync(markerId);
    }

    private string ExtractBlobNameFromUrl(string url)
    {
        try
        {
            // Extract the blob name from the full URL
            // Example: https://account.blob.core.windows.net/container/blobname -> blobname
            var uri = new Uri(url);
            var segments = uri.Segments;
            // Get the last segment and remove leading '/' if present
            return segments.Length > 0 ? segments[^1].TrimStart('/') : string.Empty;
        }
        catch (UriFormatException)
        {
            // If URL is malformed, return the original string
            return url;
        }
    }
}
