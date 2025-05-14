using FileMetadataService.Domain.Entities;
using FileMetadataService.Domain.Enums;
using SharedKernel.Interfaces;

namespace FileMetadataService.Domain.Interfaces;

public interface IFileRepository : IRepository<FileEntity>
{
    Task<IReadOnlyList<FileEntity>> GetFilesByOwnerIdAsync(Guid ownerId);
    Task<IReadOnlyList<FileEntity>> GetSharedFilesWithUserAsync(Guid userId);
    Task<IReadOnlyList<FileEntity>> GetDeletedFilesByOwnerIdAsync(Guid ownerId);
    Task<bool> UserHasAccessToFileAsync(Guid fileId, Guid userId, FilePermission requiredPermission);
    Task<int> GetNextVersionNumberAsync(Guid fileId);
}