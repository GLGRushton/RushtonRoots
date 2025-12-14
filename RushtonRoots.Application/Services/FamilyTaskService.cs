using RushtonRoots.Application.Mappers;
using RushtonRoots.Domain.UI.Models;
using RushtonRoots.Domain.UI.Requests;
using RushtonRoots.Infrastructure.Repositories;

namespace RushtonRoots.Application.Services;

public class FamilyTaskService : IFamilyTaskService
{
    private readonly IFamilyTaskRepository _taskRepository;
    private readonly IFamilyTaskMapper _mapper;
    private readonly INotificationService _notificationService;

    public FamilyTaskService(
        IFamilyTaskRepository taskRepository,
        IFamilyTaskMapper mapper,
        INotificationService notificationService)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<FamilyTaskViewModel?> GetByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task == null ? null : _mapper.MapToViewModel(task);
    }

    public async Task<List<FamilyTaskViewModel>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<FamilyTaskViewModel>> GetByHouseholdIdAsync(int householdId)
    {
        var tasks = await _taskRepository.GetByHouseholdIdAsync(householdId);
        return tasks.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<FamilyTaskViewModel>> GetByAssignedUserIdAsync(string userId)
    {
        var tasks = await _taskRepository.GetByAssignedUserIdAsync(userId);
        return tasks.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<FamilyTaskViewModel>> GetByStatusAsync(string status)
    {
        var tasks = await _taskRepository.GetByStatusAsync(status);
        return tasks.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<List<FamilyTaskViewModel>> GetByEventIdAsync(int eventId)
    {
        var tasks = await _taskRepository.GetByEventIdAsync(eventId);
        return tasks.Select(t => _mapper.MapToViewModel(t)).ToList();
    }

    public async Task<FamilyTaskViewModel> CreateTaskAsync(CreateFamilyTaskRequest request, string createdByUserId)
    {
        var task = _mapper.MapToEntity(request, createdByUserId);
        var savedTask = await _taskRepository.AddAsync(task);
        
        // Create notification for task creator
        await _notificationService.CreateNotificationAsync(
            createdByUserId,
            "Task",
            "Task Created",
            $"You created a new task: {savedTask.Title}",
            $"/tasks/{savedTask.Id}",
            savedTask.Id,
            "FamilyTask");
        
        // If assigned to someone else, notify them
        if (!string.IsNullOrEmpty(request.AssignedToUserId) && request.AssignedToUserId != createdByUserId)
        {
            await _notificationService.CreateNotificationAsync(
                request.AssignedToUserId,
                "Task",
                "Task Assigned",
                $"You have been assigned a task: {savedTask.Title}",
                $"/tasks/{savedTask.Id}",
                savedTask.Id,
                "FamilyTask");
        }
        
        return _mapper.MapToViewModel(savedTask);
    }

    public async Task<FamilyTaskViewModel> UpdateTaskAsync(int id, UpdateFamilyTaskRequest request, string userId)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        // Allow creator or assigned user to update
        if (task.CreatedByUserId != userId && task.AssignedToUserId != userId)
        {
            throw new UnauthorizedAccessException("You can only update tasks you created or are assigned to");
        }

        var wasAssignedTo = task.AssignedToUserId;
        _mapper.UpdateEntity(task, request);
        var updatedTask = await _taskRepository.UpdateAsync(task);
        
        // Notify if assignment changed
        if (request.AssignedToUserId != wasAssignedTo && !string.IsNullOrEmpty(request.AssignedToUserId))
        {
            await _notificationService.CreateNotificationAsync(
                request.AssignedToUserId,
                "Task",
                "Task Assigned",
                $"You have been assigned a task: {updatedTask.Title}",
                $"/tasks/{updatedTask.Id}",
                updatedTask.Id,
                "FamilyTask");
        }
        
        return _mapper.MapToViewModel(updatedTask);
    }

    public async Task DeleteTaskAsync(int id, string userId)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
        {
            throw new InvalidOperationException("Task not found");
        }

        if (task.CreatedByUserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete tasks you created");
        }

        await _taskRepository.DeleteAsync(id);
    }
}
