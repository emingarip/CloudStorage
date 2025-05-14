
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace FileStorageService.Domain.Interfaces
{
    public interface IFileStorageProvider
    {
        Task<Result<(string path, long size)>> StoreFileAsync(IFormFile file, Guid fileId, Guid ownerId);
        Task<Result<string>> GetFileUrlAsync(string path);
        Task<Result<string>> GetDownloadUrlAsync(string path, string fileName);
        Task<Result<Stream>> GetFileStreamAsync(string path);
        Task<Result> DeleteFileAsync(string path);
    }
}