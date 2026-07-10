using TaskTracker.Domain.Entities;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Application.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByLoginAsync(Login login);
    Task<User?> GetByEmailAsync(Email email);
    Task<User> RegisterAsync(User user);
}