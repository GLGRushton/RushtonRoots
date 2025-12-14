namespace RushtonRoots.Domain.UI.Models;

public class ContributionScoreViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int ContributionsSubmitted { get; set; }
    public int ContributionsApproved { get; set; }
    public int ContributionsRejected { get; set; }
    public int CitationsAdded { get; set; }
    public int ConflictsResolved { get; set; }
    public int PeopleAdded { get; set; }
    public int PhotosUploaded { get; set; }
    public int StoriesWritten { get; set; }
    public DateTime? LastActivityDate { get; set; }
    public string CurrentRank { get; set; } = string.Empty;
}
