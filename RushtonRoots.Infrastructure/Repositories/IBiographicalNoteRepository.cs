using RushtonRoots.Domain.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public interface IBiographicalNoteRepository
{
    Task<BiographicalNote?> GetByIdAsync(int id);
    Task<List<BiographicalNote>> GetByPersonIdAsync(int personId);
    Task<BiographicalNote> AddAsync(BiographicalNote note);
    Task<BiographicalNote> UpdateAsync(BiographicalNote note);
    Task DeleteAsync(int id);
}
