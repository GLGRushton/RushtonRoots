using RushtonRoots.Domain.Database;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;

namespace RushtonRoots.Application.Mappers;

public class FamilyTaskMapper : IFamilyTaskMapper
{
    public FamilyTaskViewModel MapToViewModel(FamilyTask familyTask)
    {
        return new FamilyTaskViewModel
        {
            Id = familyTask.Id,
            Title = familyTask.Title,
            Description = familyTask.Description,
            Status = familyTask.Status,
            Priority = familyTask.Priority,
            DueDate = familyTask.DueDate,
            CompletedDate = familyTask.CompletedDate,
            CreatedByUserId = familyTask.CreatedByUserId,
            CreatedByUserName = familyTask.CreatedByUser?.UserName,
            AssignedToUserId = familyTask.AssignedToUserId,
            AssignedToUserName = familyTask.AssignedToUser?.UserName,
            HouseholdId = familyTask.HouseholdId,
            HouseholdName = familyTask.Household?.HouseholdName,
            RelatedEventId = familyTask.RelatedEventId,
            RelatedEventTitle = familyTask.RelatedEvent?.Title,
            CreatedDateTime = familyTask.CreatedDateTime,
            UpdatedDateTime = familyTask.UpdatedDateTime
        };
    }

    public FamilyTask MapToEntity(CreateFamilyTaskRequest request, string createdByUserId)
    {
        return new FamilyTask
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            Priority = request.Priority,
            DueDate = request.DueDate,
            CreatedByUserId = createdByUserId,
            AssignedToUserId = request.AssignedToUserId,
            HouseholdId = request.HouseholdId,
            RelatedEventId = request.RelatedEventId
        };
    }

    public void UpdateEntity(FamilyTask familyTask, UpdateFamilyTaskRequest request)
    {
        familyTask.Title = request.Title;
        familyTask.Description = request.Description;
        familyTask.Status = request.Status;
        familyTask.Priority = request.Priority;
        familyTask.DueDate = request.DueDate;
        familyTask.CompletedDate = request.CompletedDate;
        familyTask.AssignedToUserId = request.AssignedToUserId;
        familyTask.HouseholdId = request.HouseholdId;
        familyTask.RelatedEventId = request.RelatedEventId;
    }
}
