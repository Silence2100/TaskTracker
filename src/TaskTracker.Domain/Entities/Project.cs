using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<TaskItem> Tasks { get; set; } = [];

    public List<ProjectMember> Members { get; set; } = [];

    public TaskItem? FindTask(int taskId)
    {
        return Tasks.FirstOrDefault(task => task.Id == taskId);
    }

    public void AddTask(TaskItem task)
    {
        task.ProjectId = Id;
        Tasks.Add(task);
    }

    public bool RemoveTask(int taskId)
    {
        var task = FindTask(taskId);

        if (task is null)
            return false;

        return Tasks.Remove(task);
    }

    public TaskItem? UpdateTaskDetails(int taskId, string title, string? description)
    {
        var task = FindTask(taskId);

        if (task is null)
            return null;

        task.Title = title.Trim();
        task.Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        task.UpdateAt = DateTime.UtcNow;

        return task;
    }

    public TaskItem? AssignTaskMember(int taskId, Guid? memberId)
    {
        var task = FindTask(taskId);

        if (task is null)
            return null;

        task.AssignedUserId = memberId;
        task.UpdateAt = DateTime.UtcNow;

        return task;
    }

    public TaskItem? ChangeTaskStatus(int taskId, TaskItemStatus status)
    {
        var task = FindTask(taskId);

        if (task is null)
            return null;

        task.Status = status;
        task.UpdateAt = DateTime.UtcNow;

        return task;
    }

    public void UnassignTasksFromUser(Guid userId)
    {
        var now = DateTime.UtcNow;

        foreach (var task in Tasks)
        {
            if (task.AssignedUserId != userId)
                continue;

            task.AssignedUserId = null;
            task.UpdateAt = now;
        }
    }
}