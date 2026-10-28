using Microsoft.EntityFrameworkCore;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;

    public MemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .Where(member => member.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .FirstOrDefaultAsync(member =>
                member.ProjectId == projectId &&
                member.UserId == userId);
    }

    public async Task<bool> IsMemberAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .AnyAsync(member =>
                member.ProjectId == projectId &&
                member.UserId == userId);
    }

    public async Task<ProjectRole?> GetRoleAsync(Guid projectId, Guid userId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .Where(member =>
                member.ProjectId == projectId &&
                member.UserId == userId)
            .Select(member => (ProjectRole?)member.Role)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasOwnerRoleAsync(Guid userId)
    {
        return await _context.ProjectMembers
            .AsNoTracking()
            .AnyAsync(member =>
                member.UserId == userId &&
                member.Role == ProjectRole.Owner);
    }

    public async Task AddAsync(ProjectMember member)
    {
        await _context.ProjectMembers.AddAsync(member);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProjectMember member)
    {
        _context.ProjectMembers.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProjectMember member)
    {
        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();
    }
}