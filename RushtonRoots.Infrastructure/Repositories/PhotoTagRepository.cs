using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class PhotoTagRepository : IPhotoTagRepository
{
    private readonly RushtonRootsDbContext _context;

    public PhotoTagRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<PhotoTag?> GetByIdAsync(int id)
    {
        return await _context.PhotoTags
            .Include(t => t.Person)
            .Include(t => t.PersonPhoto)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<PhotoTag>> GetByPhotoIdAsync(int photoId)
    {
        return await _context.PhotoTags
            .Where(t => t.PersonPhotoId == photoId)
            .Include(t => t.Person)
            .ToListAsync();
    }

    public async Task<List<PhotoTag>> GetByPersonIdAsync(int personId)
    {
        return await _context.PhotoTags
            .Where(t => t.PersonId == personId)
            .Include(t => t.PersonPhoto)
            .ToListAsync();
    }

    public async Task<PhotoTag> AddAsync(PhotoTag tag)
    {
        _context.PhotoTags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task DeleteAsync(int id)
    {
        var tag = await _context.PhotoTags.FindAsync(id);
        if (tag != null)
        {
            _context.PhotoTags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}
