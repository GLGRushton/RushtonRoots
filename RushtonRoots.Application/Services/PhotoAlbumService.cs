using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class PhotoAlbumService : IPhotoAlbumService
{
    private readonly IPhotoAlbumRepository _albumRepository;
    private readonly IPhotoAlbumMapper _mapper;

    public PhotoAlbumService(IPhotoAlbumRepository albumRepository, IPhotoAlbumMapper mapper)
    {
        _albumRepository = albumRepository;
        _mapper = mapper;
    }

    public async Task<PhotoAlbumViewModel?> GetByIdAsync(int id)
    {
        var album = await _albumRepository.GetByIdAsync(id);
        return album == null ? null : _mapper.MapToViewModel(album);
    }

    public async Task<List<PhotoAlbumViewModel>> GetAllAsync()
    {
        var albums = await _albumRepository.GetAllAsync();
        return albums.Select(a => _mapper.MapToViewModel(a)).ToList();
    }

    public async Task<List<PhotoAlbumViewModel>> GetByUserIdAsync(string userId)
    {
        var albums = await _albumRepository.GetByUserIdAsync(userId);
        return albums.Select(a => _mapper.MapToViewModel(a)).ToList();
    }

    public async Task<PhotoAlbumViewModel> CreateAlbumAsync(CreatePhotoAlbumRequest request, string userId)
    {
        var album = _mapper.MapToEntity(request, userId);
        var savedAlbum = await _albumRepository.AddAsync(album);
        return _mapper.MapToViewModel(savedAlbum);
    }

    public async Task<PhotoAlbumViewModel> UpdateAlbumAsync(int id, CreatePhotoAlbumRequest request)
    {
        var album = await _albumRepository.GetByIdAsync(id);
        if (album == null)
        {
            throw new KeyNotFoundException($"Album with ID {id} not found");
        }

        album.Name = request.Name;
        album.Description = request.Description;
        album.AlbumDate = request.AlbumDate;
        album.IsPublic = request.IsPublic;

        var updatedAlbum = await _albumRepository.UpdateAsync(album);
        return _mapper.MapToViewModel(updatedAlbum);
    }

    public async Task DeleteAlbumAsync(int id)
    {
        await _albumRepository.DeleteAsync(id);
    }
}
