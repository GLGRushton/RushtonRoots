namespace RushtonRoots.Domain.UI.Requests;

public class UpdateNotificationPreferenceRequest
{
    public string NotificationType { get; set; } = string.Empty;
    public bool InAppEnabled { get; set; }
    public bool EmailEnabled { get; set; }
    public string EmailFrequency { get; set; } = "Immediate";
}
