using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class PhotoTagService : IPhotoTagService
{
    private readonly IPhotoTagRepository _tagRepository;
    private readonly IPhotoTagMapper _mapper;

    public PhotoTagService(IPhotoTagRepository tagRepository, IPhotoTagMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<PhotoTagViewModel?> GetByIdAsync(int id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);
        return tag == null ? null : _mapper.MapToViewModel(tag);
    }

    public async Task<List<PhotoTagViewModel>> GetByPhotoIdAsync(int photoId)
    {
        var tags = await _tagRepository.GetByPhotoIdAsync(photoId);
        return tags.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<PhotoTagViewModel>> GetByPersonIdAsync(int personId)
    {
        var tags = await _tagRepository.GetByPersonIdAsync(personId);
        return tags.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<PhotoTagViewModel> CreateTagAsync(CreatePhotoTagRequest request)
    {
        var tag = _mapper.MapToEntity(request);
        var savedTag = await _tagRepository.AddAsync(tag);
        return _mapper.MapToViewModel(savedTag);
    }

    public async Task DeleteTagAsync(int id)
    {
        await _tagRepository.DeleteAsync(id);
    }
}
