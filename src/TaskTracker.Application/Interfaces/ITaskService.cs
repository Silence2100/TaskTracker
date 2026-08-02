using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Interfaces;

public interface ITaskService
{
    Task<List<TaskDto>> GetAllAsync(Guid currentUserId, UserRole currentUserRole);
    Task<TaskDto?> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole);
    Task<TaskDto?> CreateAsync(CreateTaskDto dto, Guid authorId, UserRole currentUserRole);
    Task<bool> UpdateAsync(Guid id, UpdateTaskDto dto, Guid currentUserId, UserRole currentUserRole);
    Task<bool> DeleteAsync(Guid id, Guid currentUserId, UserRole currentUserRole);
}