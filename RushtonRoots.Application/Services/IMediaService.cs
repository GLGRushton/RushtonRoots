using Microsoft.AspNetCore.Http;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IMediaService
{
    Task<MediaViewModel?> GetByIdAsync(int id);
    Task<List<MediaViewModel>> GetAllAsync();
    Task<List<MediaViewModel>> GetByUserIdAsync(string userId);
    Task<List<MediaViewModel>> GetByPersonIdAsync(int personId);
    Task<List<MediaViewModel>> SearchAsync(SearchMediaRequest request);
    Task<MediaViewModel> UploadMediaAsync(CreateMediaRequest request, IFormFile file, string userId);
    Task<MediaViewModel> UpdateMediaAsync(int id, UpdateMediaRequest request);
    Task DeleteMediaAsync(int id);
    Task<string> GetMediaStreamUrlAsync(int mediaId);
    
    // Timeline markers
    Task<MediaTimelineMarkerViewModel> AddTimelineMarkerAsync(int mediaId, CreateMediaTimelineMarkerRequest request);
    Task<List<MediaTimelineMarkerViewModel>> GetTimelineMarkersAsync(int mediaId);
    Task DeleteTimelineMarkerAsync(int markerId);
}
