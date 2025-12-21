using Microsoft.AspNetCore.Http;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;
using RushtonRoots.Infrastructure.Services;

namespace RushtonRoots.Application.Services;

public class PersonPhotoService : IPersonPhotoService
{
    private readonly IPersonPhotoRepository _photoRepository;
    private readonly IPhotoTagRepository _tagRepository;
    private readonly IBlobStorageService _blobStorageService;

    public PersonPhotoService(
        IPersonPhotoRepository photoRepository,
        IPhotoTagRepository tagRepository,
        IBlobStorageService blobStorageService)
    {
        _photoRepository = photoRepository;
        _tagRepository = tagRepository;
        _blobStorageService = blobStorageService;
    }

    public async Task<PersonPhotoViewModel?> GetByIdAsync(int id)
    {
        var photo = await _photoRepository.GetByIdAsync(id);
        if (photo == null) return null;

        var tags = await _tagRepository.GetByPhotoIdAsync(id);
        return MapToViewModel(photo, tags);
    }

    public async Task<List<PersonPhotoViewModel>> GetByPersonIdAsync(int personId)
    {
        var photos = await _photoRepository.GetByPersonIdAsync(personId);
        var viewModels = new List<PersonPhotoViewModel>();

        foreach (var photo in photos)
        {
            var tags = await _tagRepository.GetByPhotoIdAsync(photo.Id);
            viewModels.Add(MapToViewModel(photo, tags));
        }

        return viewModels;
    }

    public async Task<PersonPhotoViewModel?> GetPrimaryPhotoAsync(int personId)
    {
        var photo = await _photoRepository.GetPrimaryPhotoAsync(personId);
        if (photo == null) return null;

        var tags = await _tagRepository.GetByPhotoIdAsync(photo.Id);
        return MapToViewModel(photo, tags);
    }

    public async Task<PersonPhotoViewModel> UploadPhotoAsync(UploadPhotoRequest request, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required");
        }

        string photoUrl;
        string? thumbnailUrl = null;
        string blobName;

        using (var stream = file.OpenReadStream())
        {
            // Upload to blob storage
            photoUrl = await _blobStorageService.UploadFileAsync(
                file.FileName,
                file.ContentType,
                stream);

            // Extract blob name from URL
            blobName = ExtractBlobNameFromUrl(photoUrl);

            // Generate thumbnails for images
            if (IsImageFile(file.ContentType))
            {
                stream.Position = 0;
                try
                {
                    var thumbnails = await _blobStorageService.GenerateThumbnailsAsync(blobName, stream);
                    // Use the small thumbnail as the default thumbnail URL
                    if (thumbnails.ContainsKey("small"))
                    {
                        thumbnailUrl = thumbnails["small"];
                    }
                }
                catch
                {
                    // If thumbnail generation fails, continue without it
                }
            }
        }

        var photo = new PersonPhoto
        {
            PersonId = request.PersonId,
            PhotoAlbumId = request.PhotoAlbumId,
            PhotoUrl = photoUrl,
            ThumbnailUrl = thumbnailUrl,
            Caption = request.Caption,
            PhotoDate = request.PhotoDate,
            IsPrimary = request.IsPrimary,
            BlobName = blobName,
            FileSize = file.Length,
            ContentType = file.ContentType,
            DisplayOrder = 0
        };

        var savedPhoto = await _photoRepository.AddAsync(photo);
        return MapToViewModel(savedPhoto, new List<PhotoTag>());
    }

    public async Task<PersonPhotoViewModel> UpdatePhotoAsync(int id, CreatePersonPhotoRequest request)
    {
        var photo = await _photoRepository.GetByIdAsync(id);
        if (photo == null)
        {
            throw new KeyNotFoundException($"Photo with ID {id} not found");
        }

        photo.Caption = request.Caption;
        photo.PhotoDate = request.PhotoDate;
        photo.IsPrimary = request.IsPrimary;

        var updatedPhoto = await _photoRepository.UpdateAsync(photo);
        var tags = await _tagRepository.GetByPhotoIdAsync(id);
        return MapToViewModel(updatedPhoto, tags);
    }

    public async Task DeletePhotoAsync(int id)
    {
        var photo = await _photoRepository.GetByIdAsync(id);
        if (photo == null) return;

        // Delete from blob storage (also deletes thumbnails automatically)
        if (!string.IsNullOrEmpty(photo.BlobName))
        {
            try
            {
                await _blobStorageService.DeleteFileAsync(photo.BlobName);
            }
            catch
            {
                // Continue even if blob deletion fails
            }
        }

        await _photoRepository.DeleteAsync(id);
    }

    public async Task<List<PersonPhotoViewModel>> GetPhotosByDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        // TODO: Implement date-based filtering in repository
        // For now, return empty list as this is a future enhancement
        return await Task.FromResult(new List<PersonPhotoViewModel>());
    }

    public async Task<List<PersonPhotoViewModel>> GetPhotosByAlbumIdAsync(int albumId)
    {
        // TODO: Implement album-based filtering in repository
        // For now, return empty list as this is a future enhancement
        return await Task.FromResult(new List<PersonPhotoViewModel>());
    }

    private PersonPhotoViewModel MapToViewModel(PersonPhoto photo, List<PhotoTag> tags)
    {
        return new PersonPhotoViewModel
        {
            Id = photo.Id,
            PersonId = photo.PersonId,
            PhotoAlbumId = photo.PhotoAlbumId,
            PhotoUrl = photo.PhotoUrl,
            ThumbnailUrl = photo.ThumbnailUrl,
            Caption = photo.Caption,
            PhotoDate = photo.PhotoDate,
            IsPrimary = photo.IsPrimary,
            DisplayOrder = photo.DisplayOrder,
            FileSize = photo.FileSize,
            ContentType = photo.ContentType,
            Tags = tags.Select(t => new PhotoTagViewModel
            {
                Id = t.Id,
                PersonPhotoId = t.PersonPhotoId,
                PersonId = t.PersonId,
                PersonName = t.Person != null 
                    ? $"{t.Person.FirstName} {t.Person.LastName}".Trim() 
                    : null,
                Notes = t.Notes,
                XPosition = t.XPosition,
                YPosition = t.YPosition
            }).ToList()
        };
    }

    private string ExtractBlobNameFromUrl(string url)
    {
        var uri = new Uri(url);
        return uri.Segments.Last();
    }

    private bool IsImageFile(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
        {
            return false;
        }

        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }
}
