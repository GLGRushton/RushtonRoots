namespace RushtonRoots.Domain.UI.Requests;

public class UpdateFamilyTaskRequest
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? AssignedToUserId { get; set; }
    public int? HouseholdId { get; set; }
    public int? RelatedEventId { get; set; }
}
