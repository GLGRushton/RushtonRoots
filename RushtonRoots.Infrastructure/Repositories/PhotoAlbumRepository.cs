using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class PhotoAlbumRepository : IPhotoAlbumRepository
{
    private readonly RushtonRootsDbContext _context;

    public PhotoAlbumRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<PhotoAlbum?> GetByIdAsync(int id)
    {
        return await _context.PhotoAlbums
            .Include(a => a.Photos)
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<PhotoAlbum>> GetAllAsync()
    {
        return await _context.PhotoAlbums
            .Include(a => a.CreatedBy)
            .OrderBy(a => a.DisplayOrder)
            .ThenByDescending(a => a.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<PhotoAlbum>> GetByUserIdAsync(string userId)
    {
        return await _context.PhotoAlbums
            .Where(a => a.CreatedByUserId == userId || a.IsPublic)
            .Include(a => a.CreatedBy)
            .OrderBy(a => a.DisplayOrder)
            .ThenByDescending(a => a.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<PhotoAlbum> AddAsync(PhotoAlbum album)
    {
        _context.PhotoAlbums.Add(album);
        await _context.SaveChangesAsync();
        return album;
    }

    public async Task<PhotoAlbum> UpdateAsync(PhotoAlbum album)
    {
        _context.PhotoAlbums.Update(album);
        await _context.SaveChangesAsync();
        return album;
    }

    public async Task DeleteAsync(int id)
    {
        var album = await _context.PhotoAlbums.FindAsync(id);
        if (album != null)
        {
            _context.PhotoAlbums.Remove(album);
            await _context.SaveChangesAsync();
        }
    }
}
