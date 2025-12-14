namespace RushtonRoots.Domain.UI.Requests;

public class CreateMediaTimelineMarkerRequest
{
    public int TimeSeconds { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
}
