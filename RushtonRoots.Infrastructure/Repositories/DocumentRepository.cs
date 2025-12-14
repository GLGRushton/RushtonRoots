using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly RushtonRootsDbContext _context;

    public DocumentRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAsync(int id)
    {
        return await _context.Documents
            .Include(d => d.UploadedBy)
            .Include(d => d.Versions)
            .Include(d => d.DocumentPeople)
                .ThenInclude(dp => dp.Person)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Document>> GetAllAsync()
    {
        return await _context.Documents
            .Include(d => d.UploadedBy)
            .OrderBy(d => d.DisplayOrder)
            .ThenByDescending(d => d.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Document>> GetByUserIdAsync(string userId)
    {
        return await _context.Documents
            .Where(d => d.UploadedByUserId == userId || d.IsPublic)
            .Include(d => d.UploadedBy)
            .OrderBy(d => d.DisplayOrder)
            .ThenByDescending(d => d.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Document>> GetByCategoryAsync(string category)
    {
        return await _context.Documents
            .Where(d => d.Category == category)
            .Include(d => d.UploadedBy)
            .OrderBy(d => d.DisplayOrder)
            .ThenByDescending(d => d.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Document>> GetByPersonIdAsync(int personId)
    {
        return await _context.Documents
            .Where(d => d.DocumentPeople.Any(dp => dp.PersonId == personId))
            .Include(d => d.UploadedBy)
            .OrderBy(d => d.DisplayOrder)
            .ThenByDescending(d => d.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Document>> SearchAsync(SearchDocumentRequest request)
    {
        var query = _context.Documents.AsQueryable();

        if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(d => d.Title.Contains(request.Title));
        }

        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(d => d.Category == request.Category);
        }

        if (request.PersonId.HasValue)
        {
            query = query.Where(d => d.DocumentPeople.Any(dp => dp.PersonId == request.PersonId.Value));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(d => d.DocumentDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(d => d.DocumentDate <= request.ToDate.Value);
        }

        if (!string.IsNullOrEmpty(request.UploadedByUserId))
        {
            query = query.Where(d => d.UploadedByUserId == request.UploadedByUserId);
        }

        return await query
            .Include(d => d.UploadedBy)
            .OrderBy(d => d.DisplayOrder)
            .ThenByDescending(d => d.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<Document> AddAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document> UpdateAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task DeleteAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<DocumentVersion?> GetVersionByIdAsync(int versionId)
    {
        return await _context.DocumentVersions
            .Include(v => v.UploadedBy)
            .FirstOrDefaultAsync(v => v.Id == versionId);
    }

    public async Task<List<DocumentVersion>> GetVersionsByDocumentIdAsync(int documentId)
    {
        return await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .Include(v => v.UploadedBy)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();
    }

    public async Task<DocumentVersion> AddVersionAsync(DocumentVersion version)
    {
        _context.DocumentVersions.Add(version);
        await _context.SaveChangesAsync();
        return version;
    }

    public async Task<DocumentPerson> AddDocumentPersonAsync(DocumentPerson documentPerson)
    {
        _context.DocumentPeople.Add(documentPerson);
        await _context.SaveChangesAsync();
        return documentPerson;
    }

    public async Task RemoveDocumentPersonAsync(int documentId, int personId)
    {
        var documentPerson = await _context.DocumentPeople
            .FirstOrDefaultAsync(dp => dp.DocumentId == documentId && dp.PersonId == personId);
        
        if (documentPerson != null)
        {
            _context.DocumentPeople.Remove(documentPerson);
            await _context.SaveChangesAsync();
        }
    }
}
