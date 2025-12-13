using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

/// <summary>
/// Repository for ParentChild operations.
/// </summary>
public class ParentChildRepository : IParentChildRepository
{
    private readonly RushtonRootsDbContext _context;

    public ParentChildRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<ParentChild?> GetByIdAsync(int id)
    {
        return await _context.ParentChildren
            .Include(pc => pc.ParentPerson)
            .Include(pc => pc.ChildPerson)
            .FirstOrDefaultAsync(pc => pc.Id == id);
    }

    public async Task<IEnumerable<ParentChild>> GetAllAsync()
    {
        return await _context.ParentChildren
            .Include(pc => pc.ParentPerson)
            .Include(pc => pc.ChildPerson)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParentChild>> GetByParentIdAsync(int parentId)
    {
        return await _context.ParentChildren
            .Include(pc => pc.ParentPerson)
            .Include(pc => pc.ChildPerson)
            .Where(pc => pc.ParentPersonId == parentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ParentChild>> GetByChildIdAsync(int childId)
    {
        return await _context.ParentChildren
            .Include(pc => pc.ParentPerson)
            .Include(pc => pc.ChildPerson)
            .Where(pc => pc.ChildPersonId == childId)
            .ToListAsync();
    }

    public async Task<ParentChild> AddAsync(ParentChild parentChild)
    {
        _context.ParentChildren.Add(parentChild);
        await _context.SaveChangesAsync();
        return parentChild;
    }

    public async Task<ParentChild> UpdateAsync(ParentChild parentChild)
    {
        _context.Entry(parentChild).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return parentChild;
    }

    public async Task DeleteAsync(int id)
    {
        var parentChild = await _context.ParentChildren.FindAsync(id);
        if (parentChild != null)
        {
            _context.ParentChildren.Remove(parentChild);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasCircularRelationshipAsync(int parentId, int childId)
    {
        // Check if the proposed child is an ancestor of the proposed parent
        // This prevents circular relationships like A->B->C->A
        var visited = new HashSet<int>();
        return await IsAncestorAsync(childId, parentId, visited);
    }

    public async Task<bool> RelationshipExistsAsync(int parentId, int childId)
    {
        return await _context.ParentChildren
            .AnyAsync(pc => pc.ParentPersonId == parentId && pc.ChildPersonId == childId);
    }

    private async Task<bool> IsAncestorAsync(int potentialAncestorId, int descendantId, HashSet<int> visited)
    {
        // Prevent infinite loops
        if (visited.Contains(descendantId))
            return false;

        visited.Add(descendantId);

        // Check if potentialAncestor is directly a parent of descendant
        var parents = await _context.ParentChildren
            .Where(pc => pc.ChildPersonId == descendantId)
            .Select(pc => pc.ParentPersonId)
            .ToListAsync();

        if (parents.Contains(potentialAncestorId))
            return true;

        // Recursively check all parents
        foreach (var parentId in parents)
        {
            if (await IsAncestorAsync(potentialAncestorId, parentId, visited))
                return true;
        }

        return false;
    }
}
