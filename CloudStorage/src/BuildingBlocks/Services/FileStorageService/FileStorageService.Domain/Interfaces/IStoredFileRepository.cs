using FileStorageService.Domain.Entities;
using SharedKernel.Interfaces;

namespace FileStorageService.Domain.Interfaces;

public interface IStoredFileRepository : IRepository<StoredFile>
{
    Task<StoredFile> GetByFileIdAsync(Guid fileId);
}