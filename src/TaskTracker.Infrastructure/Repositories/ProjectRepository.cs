using Microsoft.EntityFrameworkCore;

using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Enums;
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

    public async Task<List<Project>> GetByMemberIdAsync(Guid memberId)
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(project => project.Tasks)
            .Include(project => project.Members)
            .Where(project => project.Members.Any(member => member.UserId == memberId))
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
            .Where(member => member.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<bool> IsMemberAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .AnyAsync(member => member.ProjectId == projectId && member.UserId == userId);
    }

    public async Task<ProjectMember?> GetProjectMember(Guid userId, Guid projectId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(member => member.ProjectId == projectId && member.UserId == userId);
    }

    public async Task<ProjectRole> GetUserRoleAsync(Guid userId, Guid projectId)
    {
        var member = await _context.ProjectMembers
            .AsNoTracking()
            .FirstAsync(member => member.ProjectId == projectId && member.UserId == userId);

        return member.Role;
    }

    public async Task<bool> HasOwnerRoleAsync(Guid userId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .AnyAsync(member => member.UserId == userId && member.Role == ProjectRole.Owner);
    }
}