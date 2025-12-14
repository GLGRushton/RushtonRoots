namespace RushtonRoots.Domain.UI.Requests;

public class CreateFamilyTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Pending";
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
    public string? AssignedToUserId { get; set; }
    public int? HouseholdId { get; set; }
    public int? RelatedEventId { get; set; }
}
