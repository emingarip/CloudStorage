using System.Linq.Expressions;
using FileMetadataService.Domain.Entities;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using FileMetadataService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileMetadataService.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly FileMetadataDbContext _context;

    public FileRepository(FileMetadataDbContext context)
    {
        _context = context;
    }

    public async Task<FileEntity> GetByIdAsync(Guid id)
    {
        return await _context.Files
            .Include(f => f.Shares)
            .Include(f => f.Versions)
            .Include(f => f.Activities)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IReadOnlyList<FileEntity>> ListAllAsync()
    {
        return await _context.Files
            .Include(f => f.Shares)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FileEntity>> ListAsync(Expression<Func<FileEntity, bool>> predicate)
    {
        return await _context.Files
            .Include(f => f.Shares)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<FileEntity> AddAsync(FileEntity entity)
    {
        await _context.Files.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(FileEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(FileEntity entity)
    {
        _context.Files.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<FileEntity>> GetFilesByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Files
            .Include(f => f.Shares)
            .Where(f => f.OwnerId == ownerId && f.Status == FileStatus.Active)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FileEntity>> GetSharedFilesWithUserAsync(Guid userId)
    {
        return await _context.Files
            .Include(f => f.Shares)
            .Where(f => f.Status == FileStatus.Active && f.Shares.Any(s => s.UserId == userId && s.IsActive))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<FileEntity>> GetDeletedFilesByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Files
            .Include(f => f.Shares)
            .Where(f => f.OwnerId == ownerId && f.Status == FileStatus.Deleted)
            .ToListAsync();
    }

    public async Task<bool> UserHasAccessToFileAsync(Guid fileId, Guid userId, FilePermission requiredPermission)
    {
        var file = await _context.Files
            .Include(f => f.Shares)
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (file == null)
            return false;

        return file.CanUserAccess(userId, requiredPermission);
    }

    public async Task<int> GetNextVersionNumberAsync(Guid fileId)
    {
        var maxVersion = await _context.FileVersions
            .Where(v => v.FileId == fileId)
            .MaxAsync(v => (int?)v.VersionNumber) ?? 0;

        return maxVersion + 1;
    }
}