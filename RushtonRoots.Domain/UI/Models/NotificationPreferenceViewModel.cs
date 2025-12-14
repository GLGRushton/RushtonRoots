namespace RushtonRoots.Domain.UI.Models;

public class NotificationPreferenceViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public bool InAppEnabled { get; set; }
    public bool EmailEnabled { get; set; }
    public string EmailFrequency { get; set; } = "Immediate";
}
