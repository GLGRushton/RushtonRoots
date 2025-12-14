using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly RushtonRootsDbContext _context;

    public MediaRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<Media?> GetByIdAsync(int id)
    {
        return await _context.Media
            .Include(m => m.UploadedBy)
            .Include(m => m.TimelineMarkers)
            .Include(m => m.MediaPeople)
                .ThenInclude(mp => mp.Person)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Media>> GetAllAsync()
    {
        return await _context.Media
            .Include(m => m.UploadedBy)
            .OrderBy(m => m.DisplayOrder)
            .ThenByDescending(m => m.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Media>> GetByUserIdAsync(string userId)
    {
        return await _context.Media
            .Where(m => m.UploadedByUserId == userId || m.IsPublic)
            .Include(m => m.UploadedBy)
            .OrderBy(m => m.DisplayOrder)
            .ThenByDescending(m => m.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Media>> GetByMediaTypeAsync(MediaType mediaType)
    {
        return await _context.Media
            .Where(m => m.MediaType == mediaType)
            .Include(m => m.UploadedBy)
            .OrderBy(m => m.DisplayOrder)
            .ThenByDescending(m => m.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Media>> GetByPersonIdAsync(int personId)
    {
        return await _context.Media
            .Where(m => m.MediaPeople.Any(mp => mp.PersonId == personId))
            .Include(m => m.UploadedBy)
            .OrderBy(m => m.DisplayOrder)
            .ThenByDescending(m => m.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<List<Media>> SearchAsync(SearchMediaRequest request)
    {
        var query = _context.Media.AsQueryable();

        if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(m => m.Title.Contains(request.Title));
        }

        if (request.MediaType.HasValue)
        {
            query = query.Where(m => m.MediaType == request.MediaType.Value);
        }

        if (request.PersonId.HasValue)
        {
            query = query.Where(m => m.MediaPeople.Any(mp => mp.PersonId == request.PersonId.Value));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(m => m.MediaDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(m => m.MediaDate <= request.ToDate.Value);
        }

        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(m => m.UploadedByUserId == request.UserId);
        }

        if (request.HasTranscription.HasValue)
        {
            if (request.HasTranscription.Value)
            {
                query = query.Where(m => !string.IsNullOrEmpty(m.Transcription));
            }
            else
            {
                query = query.Where(m => string.IsNullOrEmpty(m.Transcription));
            }
        }

        return await query
            .Include(m => m.UploadedBy)
            .OrderBy(m => m.DisplayOrder)
            .ThenByDescending(m => m.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<Media> AddAsync(Media media)
    {
        _context.Media.Add(media);
        await _context.SaveChangesAsync();
        return media;
    }

    public async Task<Media> UpdateAsync(Media media)
    {
        _context.Media.Update(media);
        await _context.SaveChangesAsync();
        return media;
    }

    public async Task DeleteAsync(int id)
    {
        var media = await _context.Media.FindAsync(id);
        if (media != null)
        {
            _context.Media.Remove(media);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<MediaTimelineMarker?> GetMarkerByIdAsync(int markerId)
    {
        return await _context.MediaTimelineMarkers
            .FirstOrDefaultAsync(m => m.Id == markerId);
    }

    public async Task<List<MediaTimelineMarker>> GetMarkersByMediaIdAsync(int mediaId)
    {
        return await _context.MediaTimelineMarkers
            .Where(m => m.MediaId == mediaId)
            .OrderBy(m => m.TimeSeconds)
            .ToListAsync();
    }

    public async Task<MediaTimelineMarker> AddMarkerAsync(MediaTimelineMarker marker)
    {
        _context.MediaTimelineMarkers.Add(marker);
        await _context.SaveChangesAsync();
        return marker;
    }

    public async Task<MediaTimelineMarker> UpdateMarkerAsync(MediaTimelineMarker marker)
    {
        _context.MediaTimelineMarkers.Update(marker);
        await _context.SaveChangesAsync();
        return marker;
    }

    public async Task DeleteMarkerAsync(int markerId)
    {
        var marker = await _context.MediaTimelineMarkers.FindAsync(markerId);
        if (marker != null)
        {
            _context.MediaTimelineMarkers.Remove(marker);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<MediaPerson> AddMediaPersonAsync(MediaPerson mediaPerson)
    {
        _context.MediaPeople.Add(mediaPerson);
        await _context.SaveChangesAsync();
        return mediaPerson;
    }

    public async Task RemoveMediaPersonAsync(int mediaId, int personId)
    {
        var mediaPerson = await _context.MediaPeople
            .FirstOrDefaultAsync(mp => mp.MediaId == mediaId && mp.PersonId == personId);
        
        if (mediaPerson != null)
        {
            _context.MediaPeople.Remove(mediaPerson);
            await _context.SaveChangesAsync();
        }
    }
}
