using Microsoft.EntityFrameworkCore;
using RushtonRoots.Domain.Database;
using RushtonRoots.Infrastructure.Database;

namespace RushtonRoots.Infrastructure.Repositories;

public class ContributionScoreRepository : IContributionScoreRepository
{
    private readonly RushtonRootsDbContext _context;

    public ContributionScoreRepository(RushtonRootsDbContext context)
    {
        _context = context;
    }

    public async Task<ContributionScore?> GetByIdAsync(int id)
    {
        return await _context.ContributionScores
            .Include(cs => cs.User)
            .FirstOrDefaultAsync(cs => cs.Id == id);
    }

    public async Task<ContributionScore?> GetByUserIdAsync(string userId)
    {
        return await _context.ContributionScores
            .Include(cs => cs.User)
            .FirstOrDefaultAsync(cs => cs.UserId == userId);
    }

    public async Task<IEnumerable<ContributionScore>> GetLeaderboardAsync(int count = 10)
    {
        return await _context.ContributionScores
            .Include(cs => cs.User)
            .OrderByDescending(cs => cs.TotalPoints)
            .Take(count)
            .ToListAsync();
    }

    public async Task<ContributionScore> CreateAsync(ContributionScore score)
    {
        _context.ContributionScores.Add(score);
        await _context.SaveChangesAsync();
        return score;
    }

    public async Task<ContributionScore> UpdateAsync(ContributionScore score)
    {
        _context.ContributionScores.Update(score);
        await _context.SaveChangesAsync();
        return score;
    }

    public async Task<ContributionScore> GetOrCreateScoreAsync(string userId)
    {
        var score = await GetByUserIdAsync(userId);
        if (score == null)
        {
            score = new ContributionScore
            {
                UserId = userId,
                TotalPoints = 0,
                CurrentRank = "Novice"
            };
            await CreateAsync(score);
        }
        return score;
    }
}
