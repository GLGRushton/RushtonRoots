using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IPersonPhotoRepository
{
    Task<PersonPhoto?> GetByIdAsync(int id);
    Task<List<PersonPhoto>> GetByPersonIdAsync(int personId);
    Task<PersonPhoto?> GetPrimaryPhotoAsync(int personId);
    Task<PersonPhoto> AddAsync(PersonPhoto photo);
    Task<PersonPhoto> UpdateAsync(PersonPhoto photo);
    Task DeleteAsync(int id);
}
