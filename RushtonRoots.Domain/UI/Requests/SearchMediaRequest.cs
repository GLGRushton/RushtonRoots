using RushtonRoots.Domain.Database;

namespace RushtonRoots.Domain.UI.Requests;

public class SearchMediaRequest
{
    public string? Title { get; set; }
    public MediaType? MediaType { get; set; }
    public int? PersonId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? UserId { get; set; }
    public bool? HasTranscription { get; set; }
}
