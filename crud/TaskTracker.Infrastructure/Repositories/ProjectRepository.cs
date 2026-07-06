using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;
public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(project => project.Tasks)
            .Include(project => project.Members)
            .OrderBy(project => project.Name)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(project => project.Tasks)
            .Include(project => project.Members)
            .FirstOrDefaultAsync(project => project.Id == id);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(project.Id) ?? project;
    }

    public async Task<List<ProjectMember>> GetMembersAsync(Guid projectId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .Include(member => member.User)
            .Where(member => member.ProjectId == projectId)
            .OrderBy(member => member.User.Name)
            .ToListAsync();
    }
}