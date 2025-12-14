using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class BiographicalNoteRepository : IBiographicalNoteRepository
{
    private readonly RushtonRootsDbContext _context;

    public BiographicalNoteRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<BiographicalNote?> GetByIdAsync(int id)
    {
        return await _context.BiographicalNotes
            .Include(n => n.Source)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<List<BiographicalNote>> GetByPersonIdAsync(int personId)
    {
        return await _context.BiographicalNotes
            .Include(n => n.Source)
            .Where(n => n.PersonId == personId)
            .OrderByDescending(n => n.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<BiographicalNote> AddAsync(BiographicalNote note)
    {
        _context.BiographicalNotes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task<BiographicalNote> UpdateAsync(BiographicalNote note)
    {
        _context.BiographicalNotes.Update(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task DeleteAsync(int id)
    {
        var note = await _context.BiographicalNotes.FindAsync(id);
        if (note != null)
        {
            _context.BiographicalNotes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}
