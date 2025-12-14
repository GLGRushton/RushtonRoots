using Microsoft.AspNetCore.Http;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Services;

public interface IPersonPhotoService
{
    Task<PersonPhotoViewModel?> GetByIdAsync(int id);
    Task<List<PersonPhotoViewModel>> GetByPersonIdAsync(int personId);
    Task<PersonPhotoViewModel?> GetPrimaryPhotoAsync(int personId);
    Task<PersonPhotoViewModel> UploadPhotoAsync(UploadPhotoRequest request, IFormFile file);
    Task<PersonPhotoViewModel> UpdatePhotoAsync(int id, CreatePersonPhotoRequest request);
    Task DeletePhotoAsync(int id);
    Task<List<PersonPhotoViewModel>> GetPhotosByDateRangeAsync(DateTime? startDate, DateTime? endDate);
    Task<List<PersonPhotoViewModel>> GetPhotosByAlbumIdAsync(int albumId);
}
