using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FileMetadataService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FileMetadataService.Infrastructure.Services
{
    public class StorageService : IStorageService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StorageService> _logger;

        public StorageService(IHttpClientFactory httpClientFactory, ILogger<StorageService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Result<string>> GetFileUrlAsync(Guid fileId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("StorageService");
                var response = await client.GetAsync($"/api/storage/{fileId}/url");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StorageResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return Result.Success(result.Url);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error getting file URL: {Error}", error);
                    return Result.Failure<string>($"Error getting file URL: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file URL");
                return Result.Failure<string>($"Error getting file URL: {ex.Message}");
            }
        }

        public async Task<Result<string>> GetDownloadUrlAsync(Guid fileId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("StorageService");
                var response = await client.GetAsync($"/api/storage/{fileId}/download");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<StorageResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return Result.Success(result.Url);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error getting download URL: {Error}", error);
                    return Result.Failure<string>($"Error getting download URL: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting download URL");
                return Result.Failure<string>($"Error getting download URL: {ex.Message}");
            }
        }

        private class StorageResponse
        {
            public string Url { get; set; }
        }
    }
}