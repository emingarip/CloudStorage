using System.Linq.Expressions;
using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Interfaces;
using FileStorageService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Repositories;

public class StoredFileRepository : IStoredFileRepository
{
    private readonly FileStorageDbContext _context;

    public StoredFileRepository(FileStorageDbContext context)
    {
        _context = context;
    }

    public async Task<StoredFile> GetByIdAsync(Guid id)
    {
        return await _context.StoredFiles.FindAsync(id);
    }

    public async Task<StoredFile> GetByFileIdAsync(Guid fileId)
    {
        return await _context.StoredFiles
            .FirstOrDefaultAsync(f => f.FileId == fileId);
    }

    public async Task<IReadOnlyList<StoredFile>> ListAllAsync()
    {
        return await _context.StoredFiles.ToListAsync();
    }

    public async Task<IReadOnlyList<StoredFile>> ListAsync(Expression<Func<StoredFile, bool>> predicate)
    {
        return await _context.StoredFiles
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<StoredFile> AddAsync(StoredFile entity)
    {
        await _context.StoredFiles.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(StoredFile entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(StoredFile entity)
    {
        _context.StoredFiles.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}