using System.Linq.Expressions;
using AuthService.Domain;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IReadOnlyList<User>> ListAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<IReadOnlyList<User>> ListAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users.Where(predicate).ToListAsync();
    }

    public async Task<User> AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(User entity)
    {
        // Attach the entity if it's not being tracked
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            var existingEntity = await _context.Users.FindAsync(entity.Id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        var _user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        return _user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}