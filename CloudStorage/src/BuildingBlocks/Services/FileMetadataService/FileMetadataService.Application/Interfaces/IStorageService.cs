using SharedKernel;

namespace FileMetadataService.Application.Interfaces;

public interface IStorageService
{
    Task<Result<string>> GetFileUrlAsync(Guid fileId);
    Task<Result<string>> GetDownloadUrlAsync(Guid fileId);
}