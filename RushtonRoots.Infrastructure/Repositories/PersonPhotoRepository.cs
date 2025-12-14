using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class PersonPhotoRepository : IPersonPhotoRepository
{
    private readonly RushtonRootsDbContext _context;

    public PersonPhotoRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<PersonPhoto?> GetByIdAsync(int id)
    {
        return await _context.PersonPhotos.FindAsync(id);
    }

    public async Task<List<PersonPhoto>> GetByPersonIdAsync(int personId)
    {
        return await _context.PersonPhotos
            .Where(p => p.PersonId == personId)
            .OrderByDescending(p => p.IsPrimary)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<PersonPhoto?> GetPrimaryPhotoAsync(int personId)
    {
        return await _context.PersonPhotos
            .FirstOrDefaultAsync(p => p.PersonId == personId && p.IsPrimary);
    }

    public async Task<PersonPhoto> AddAsync(PersonPhoto photo)
    {
        // If this is set as primary, unset any existing primary photo
        if (photo.IsPrimary)
        {
            var existingPrimary = await GetPrimaryPhotoAsync(photo.PersonId);
            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
                _context.PersonPhotos.Update(existingPrimary);
            }
        }

        _context.PersonPhotos.Add(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task<PersonPhoto> UpdateAsync(PersonPhoto photo)
    {
        // If this is set as primary, unset any existing primary photo
        if (photo.IsPrimary)
        {
            var existingPrimary = await _context.PersonPhotos
                .FirstOrDefaultAsync(p => p.PersonId == photo.PersonId && p.IsPrimary && p.Id != photo.Id);
            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
                _context.PersonPhotos.Update(existingPrimary);
            }
        }

        _context.PersonPhotos.Update(photo);
        await _context.SaveChangesAsync();
        return photo;
    }

    public async Task DeleteAsync(int id)
    {
        var photo = await _context.PersonPhotos.FindAsync(id);
        if (photo != null)
        {
            _context.PersonPhotos.Remove(photo);
            await _context.SaveChangesAsync();
        }
    }
}
