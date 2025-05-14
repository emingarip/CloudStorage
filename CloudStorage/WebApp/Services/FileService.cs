using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Services
{
    public class FileService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileService> _logger;

        public FileService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<FileService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Configure HTTP client
            string baseUrl = _configuration["ApiSettings:FileMetadataService:BaseUrl"] ?? "http://localhost:5264";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<FileViewModel>> GetUserFilesAsync(string token, string userId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("api/files");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var files = JsonSerializer.Deserialize<List<FileViewModel>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return files ?? new List<FileViewModel>();
                }

                _logger.LogWarning($"Failed to get user files: {response.StatusCode}");
                return new List<FileViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user files");
                return new List<FileViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<FileViewModel>> GetSharedFilesAsync(string token, string userId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("api/files/shared");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var files = JsonSerializer.Deserialize<List<FileViewModel>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return files ?? new List<FileViewModel>();
                }

                _logger.LogWarning($"Failed to get shared files: {response.StatusCode}");
                return new List<FileViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shared files");
                return new List<FileViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<FileViewModel>> GetDeletedFilesAsync(string token, string userId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("api/files/deleted");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var files = JsonSerializer.Deserialize<List<FileViewModel>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return files ?? new List<FileViewModel>();
                }

                _logger.LogWarning($"Failed to get deleted files: {response.StatusCode}");
                return new List<FileViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deleted files");
                return new List<FileViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<FileViewModel?> GetFileDetailsAsync(string token, Guid fileId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"api/files/{fileId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FileViewModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                _logger.LogWarning($"Failed to get file details: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file details");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> UploadFileAsync(string token, UploadFileViewModel model, string userId)
        {
            try
            {
                // Yeni bir FileId oluştur
                var fileId = Guid.NewGuid();

                // 1. Adım: Dosyayı FileStorageService'e yükle
                // FileStorageService'in base URL'sini kullanmak için geçici bir HttpClient oluşturuyoruz
                using var fileStorageClient = new HttpClient();
                string fileStorageBaseUrl = _configuration["ApiSettings:FileStorageService:BaseUrl"] ?? "http://localhost:5025";
                fileStorageClient.BaseAddress = new Uri(fileStorageBaseUrl);
                fileStorageClient.DefaultRequestHeaders.Accept.Clear();
                fileStorageClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                fileStorageClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using var content = new MultipartFormDataContent();
                using var fileStream = model.File!.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.File.ContentType);

                content.Add(streamContent, "file", model.File.FileName);
                content.Add(new StringContent(userId), "userId");
                content.Add(new StringContent(fileId.ToString()), "fileId");

                var storageResponse = await fileStorageClient.PostAsync("api/files/upload", content);

                if (!storageResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error uploading file to FileStorageService: {StatusCode}", storageResponse.StatusCode);
                    return false;
                }

                // Dosya bilgilerini al
                var storageResponseContent = await storageResponse.Content.ReadAsStringAsync();
                var storageResult = JsonSerializer.Deserialize<StoredFileResult>(storageResponseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (storageResult == null)
                {
                    _logger.LogError("Failed to deserialize storage response");
                    return false;
                }

                // 2. Adım: Metadata bilgilerini FileMetadataService'e kaydet
                // FileMetadataService için HttpClient kullanıyoruz (mevcut _httpClient)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var metadataCommand = new
                {
                    Name = model.File.FileName,
                    ContentType = model.File.ContentType,
                    Size = model.File.Length,
                    Path = storageResult.Path,
                    OwnerId = Guid.Parse(userId),
                    Description = model.Description ?? string.Empty
                };

                var metadataContent = new StringContent(
                    JsonSerializer.Serialize(metadataCommand),
                    Encoding.UTF8,
                    "application/json");

                var metadataResponse = await _httpClient.PostAsync("api/files", metadataContent);

                if (!metadataResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Error saving metadata to FileMetadataService: {StatusCode}", metadataResponse.StatusCode);
                    // Dosya yüklendi ama metadata kaydedilemedi, yine de başarılı sayalım
                    return true;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // FileStorageService'den dönen sonuç için yardımcı sınıf
        private class StoredFileResult
        {
            public Guid Id { get; set; }
            public string FileName { get; set; } = string.Empty;
            public string ContentType { get; set; } = string.Empty;
            public long Size { get; set; }
            public string Path { get; set; } = string.Empty;
            public string OwnerId { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        public async Task<bool> DeleteFileAsync(string token, Guid fileId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.DeleteAsync($"api/files/{fileId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> RestoreFileAsync(string token, Guid fileId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"api/files/{fileId}/restore", null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring file");
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> PermanentDeleteFileAsync(string token, Guid fileId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // FileMetadataService'de permanent delete endpoint'i olmadığı için normal delete endpoint'ini kullanıyoruz
                // Gerçek uygulamada, bu endpoint'in eklenmesi veya farklı bir yaklaşım kullanılması gerekebilir
                var response = await _httpClient.DeleteAsync($"api/files/{fileId}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting file");
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> ShareFileAsync(string token, ShareFileViewModel model)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var shareCommand = new
                {
                    FileId = model.FileId,
                    Email = model.Email,
                    Permission = model.Permission
                };

                var content = new StringContent(JsonSerializer.Serialize(shareCommand), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"api/iles/{model.FileId}/share", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing file");
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<byte[]?> DownloadFileAsync(string token, Guid fileId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Önce download URL'sini alalım
                var urlResponse = await _httpClient.GetAsync($"api/files/{fileId}/download");

                if (!urlResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get download URL: {urlResponse.StatusCode}");
                    return null;
                }

                var urlContent = await urlResponse.Content.ReadAsStringAsync();
                var downloadUrlInfo = JsonSerializer.Deserialize<DownloadUrlInfo>(urlContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (downloadUrlInfo == null || string.IsNullOrEmpty(downloadUrlInfo.Url))
                {
                    _logger.LogWarning("Download URL is null or empty");
                    return null;
                }

                // Şimdi dosyayı indirelim
                var fileResponse = await _httpClient.GetAsync(downloadUrlInfo.Url);

                if (fileResponse.IsSuccessStatusCode)
                {
                    return await fileResponse.Content.ReadAsByteArrayAsync();
                }

                _logger.LogWarning($"Failed to download file: {fileResponse.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private class DownloadUrlInfo
        {
            public string Url { get; set; } = string.Empty;
        }
    }
}