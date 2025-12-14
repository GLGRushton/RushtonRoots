namespace RushtonRoots.Domain.UI.Requests;

public class CreateContributionRequest
{
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string NewValue { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public int? CitationId { get; set; }
}
