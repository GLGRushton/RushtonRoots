using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly RushtonRootsDbContext _context;

    public SourceRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Source?> GetByIdAsync(int id)
    {
        return await _context.Sources
            .Include(s => s.Citations)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Source>> GetAllAsync()
    {
        return await _context.Sources
            .OrderBy(s => s.Title)
            .ToListAsync();
    }

    public async Task<List<Source>> SearchAsync(string searchTerm)
    {
        return await _context.Sources
            .Where(s => s.Title.Contains(searchTerm) ||
                       (s.Author != null && s.Author.Contains(searchTerm)) ||
                       (s.Publisher != null && s.Publisher.Contains(searchTerm)))
            .OrderBy(s => s.Title)
            .ToListAsync();
    }

    public async Task<Source> AddAsync(Source source)
    {
        _context.Sources.Add(source);
        await _context.SaveChangesAsync();
        return source;
    }

    public async Task<Source> UpdateAsync(Source source)
    {
        _context.Sources.Update(source);
        await _context.SaveChangesAsync();
        return source;
    }

    public async Task DeleteAsync(int id)
    {
        var source = await _context.Sources.FindAsync(id);
        if (source != null)
        {
            _context.Sources.Remove(source);
            await _context.SaveChangesAsync();
        }
    }
}
