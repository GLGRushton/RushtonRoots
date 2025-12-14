namespace RushtonRoots.Domain.UI.Requests;

public class ResolveConflictRequest
{
    public int ConflictId { get; set; }
    public string Resolution { get; set; } = string.Empty; // AcceptCurrent, AcceptNew, AcceptBoth, Custom
    public string? ResolutionNotes { get; set; }
    public int? AcceptedCitationId { get; set; }
}
