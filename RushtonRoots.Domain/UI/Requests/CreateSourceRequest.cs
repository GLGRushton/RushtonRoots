namespace RushtonRoots.Domain.UI.Requests;

public class CreateSourceRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? RepositoryName { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? CallNumber { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
