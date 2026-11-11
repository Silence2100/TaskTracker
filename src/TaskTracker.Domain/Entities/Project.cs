using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Project
{
    private readonly List<TaskItem> _tasks = [];
    private readonly List<ProjectMember> _members = [];

    public Guid Id { get; set; }

    public string Name { get; private set; }

    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

    private Project()
    {
        Name = string.Empty;
    }

    public Project(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200)
            throw new ArgumentException("Invalid project name.", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
    }

    public ProjectMember? FindMember(Guid userId)
    {
        return _members.FirstOrDefault(member => member.UserId == userId);
    }

    public bool IsMember(Guid? userId)
    {
        if (userId is null)
            return false;

        return FindMember(userId.Value) is not null;
    }

    public ProjectRole? GetMemberRole(Guid userId)
    {
        return FindMember(userId)?.Role;
    }

    public bool IsLastOwner(Guid userId)
    {
        var member = FindMember(userId);

        if (member is null || member.Role != ProjectRole.Owner)
            return false;

        return _members.Count(item => item.Role == ProjectRole.Owner) == 1;
    }

    public ProjectMember? AddMember(Guid userId, ProjectRole role)
    {
        if (FindMember(userId) is not null)
            return null;

        var member = new ProjectMember
        {
            ProjectId = Id,
            UserId = userId,
            Role = role
        };

        _members.Add(member);

        return member;
    }

    public bool RemoveMember(Guid userId)
    {
        var member = FindMember(userId);

        if (member is null || IsLastOwner(userId))
            return false;

        if (!_members.Remove(member))
            return false;

        UnassignTasksFromUser(userId);

        return true;
    }

    public ProjectMember? UpdateMemberRole(Guid userId, ProjectRole role)
    {
        var member = FindMember(userId);

        if (member is null)
            return null;

        if (IsLastOwner(userId) && role != ProjectRole.Owner)
            return null;

        member.Role = role;

        return member;
    }

    public TaskItem? FindTask(int taskId)
    {
        return _tasks.FirstOrDefault(task => task.Id == taskId);
    }

    public void AddTask(TaskItem task)
    {
        task.ProjectId = Id;
        _tasks.Add(task);
    }

    public bool RemoveTask(int taskId)
    {
        var task = FindTask(taskId);

        if (task is null)
            return false;

        return _tasks.Remove(task);
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

        foreach (var task in _tasks)
        {
            if (task.AssignedUserId != userId)
                continue;

            task.AssignedUserId = null;
            task.UpdateAt = now;
        }
    }
}