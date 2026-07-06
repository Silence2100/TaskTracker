using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappings;
using TaskTracker.Application.Services;

namespace TaskTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile<MappingProfile>();
        });

        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();

        return services;
    }
}