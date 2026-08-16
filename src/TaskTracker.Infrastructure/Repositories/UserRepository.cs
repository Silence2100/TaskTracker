using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskTracker.Application.Common;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.ValueObjects;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ToListAsync();
    }

    public async Task<User?> ReadByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetByLoginAsync(Login login)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Login == login);
    }

    public async Task<bool> HasLoginAsync(Login login)
    {
        return await _context.Users
            .AnyAsync(user => user.Login == login);
    }

    public async Task<bool> HasEmailAsync(Email email)
    {
        return await _context.Users
            .AnyAsync(user => user.Email == email);
    }

    public async Task RegisterAsync(User user)
    {
        await _context.Users
            .AddAsync(user);

        await _context
            .SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await ReadByIdAsync(id);

        if (user is null)
            return false;

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }
}