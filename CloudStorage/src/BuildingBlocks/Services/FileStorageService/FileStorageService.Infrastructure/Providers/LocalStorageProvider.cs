using FileStorageService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel;

namespace FileStorageService.Infrastructure.Providers;

public class LocalStorageProvider : IFileStorageProvider
{
    private readonly LocalStorageSettings _settings;
    private readonly ILogger<LocalStorageProvider> _logger;

    public LocalStorageProvider(IOptions<LocalStorageSettings> settings, ILogger<LocalStorageProvider> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        // Ensure storage directory exists
        if (!Directory.Exists(_settings.BasePath))
        {
            Directory.CreateDirectory(_settings.BasePath);
        }
    }

    public async Task<Result<(string path, long size)>> StoreFileAsync(IFormFile file, Guid fileId, Guid ownerId)
    {
        try
        {
            // Create directory structure: BasePath/OwnerId/FileId
            var ownerDir = Path.Combine(_settings.BasePath, ownerId.ToString());
            if (!Directory.Exists(ownerDir))
            {
                Directory.CreateDirectory(ownerDir);
            }

            var fileDir = Path.Combine(ownerDir, fileId.ToString());
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(fileDir, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path (for storage in database)
            var relativePath = Path.Combine(ownerId.ToString(), fileId.ToString(), fileName);

            return Result.Success((relativePath, file.Length));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing file {FileName} for owner {OwnerId}", file.FileName, ownerId);
            return Result.Failure<(string, long)>($"Error storing file: {ex.Message}");
        }
    }

    public Task<Result<string>> GetFileUrlAsync(string path)
    {
        try
        {
            // Combine base URL with relative path
            var url = $"{_settings.BaseUrl}/{path.Replace('\\', '/')}";
            return Task.FromResult(Result.Success(url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting URL for file {Path}", path);
            return Task.FromResult(Result.Failure<string>($"Error getting file URL: {ex.Message}"));
        }
    }

    public Task<Result<string>> GetDownloadUrlAsync(string path, string fileName)
    {
        try
        {
            // For local storage, we'll use the same URL but add a download parameter
            var url = $"{_settings.BaseUrl}/{path.Replace('\\', '/')}?download={Uri.EscapeDataString(fileName)}";
            return Task.FromResult(Result.Success(url));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting download URL for file {Path}", path);
            return Task.FromResult(Result.Failure<string>($"Error getting download URL: {ex.Message}"));
        }
    }

    public Task<Result<Stream>> GetFileStreamAsync(string path)
    {
        try
        {
            var fullPath = Path.Combine(_settings.BasePath, path);
            if (!File.Exists(fullPath))
            {
                return Task.FromResult(Result.Failure<Stream>("File not found"));
            }

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(Result.Success(stream as Stream));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file stream for {Path}", path);
            return Task.FromResult(Result.Failure<Stream>($"Error getting file stream: {ex.Message}"));
        }
    }

    public Task<Result> DeleteFileAsync(string path)
    {
        try
        {
            var fullPath = Path.Combine(_settings.BasePath, path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {Path}", path);
            return Task.FromResult(Result.Failure($"Error deleting file: {ex.Message}"));
        }
    }
}