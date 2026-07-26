using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}