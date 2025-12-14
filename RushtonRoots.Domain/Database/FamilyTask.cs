namespace RushtonRoots.Domain.Database;

/// <summary>
/// Represents a task for family projects
/// </summary>
public class FamilyTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, InProgress, Completed, Cancelled
    public string Priority { get; set; } = string.Empty; // Low, Medium, High, Urgent
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? AssignedToUserId { get; set; }
    public int? HouseholdId { get; set; }
    public int? RelatedEventId { get; set; }

    // Navigation properties
    public ApplicationUser? CreatedByUser { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }
    public Household? Household { get; set; }
    public FamilyEvent? RelatedEvent { get; set; }
}
