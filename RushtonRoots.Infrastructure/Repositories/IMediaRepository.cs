using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IMediaRepository
{
    Task<Media?> GetByIdAsync(int id);
    Task<List<Media>> GetAllAsync();
    Task<List<Media>> GetByUserIdAsync(string userId);
    Task<List<Media>> GetByMediaTypeAsync(MediaType mediaType);
    Task<List<Media>> GetByPersonIdAsync(int personId);
    Task<List<Media>> SearchAsync(SearchMediaRequest request);
    Task<Media> AddAsync(Media media);
    Task<Media> UpdateAsync(Media media);
    Task DeleteAsync(int id);
    
    // Timeline markers
    Task<MediaTimelineMarker?> GetMarkerByIdAsync(int markerId);
    Task<List<MediaTimelineMarker>> GetMarkersByMediaIdAsync(int mediaId);
    Task<MediaTimelineMarker> AddMarkerAsync(MediaTimelineMarker marker);
    Task<MediaTimelineMarker> UpdateMarkerAsync(MediaTimelineMarker marker);
    Task DeleteMarkerAsync(int markerId);
    
    // Media-Person associations
    Task<MediaPerson> AddMediaPersonAsync(MediaPerson mediaPerson);
    Task RemoveMediaPersonAsync(int mediaId, int personId);
}
