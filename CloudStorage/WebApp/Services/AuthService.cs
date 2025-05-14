using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Configure HTTP client
            string baseUrl = _configuration["ApiSettings:AuthService:BaseUrl"] ?? "http://localhost:5024";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginViewModel model)
        {
            try
            {
                var loginCommand = new
                {
                    Username = model.Username,
                    Password = model.Password,
                    IpAddress = "127.0.0.1" // Simplified for demo
                };

                var content = new StringContent(JsonSerializer.Serialize(loginCommand), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                _logger.LogWarning($"Login failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return null;
            }
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var registerCommand = new
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    IpAddress = "127.0.0.1" // Simplified for demo
                };

                var content = new StringContent(JsonSerializer.Serialize(registerCommand), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                _logger.LogWarning($"Registration failed: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return null;
            }
        }

        public async Task<bool> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var refreshCommand = new
                {
                    RefreshToken = refreshToken,
                    IpAddress = "127.0.0.1" // Simplified for demo
                };

                var content = new StringContent(JsonSerializer.Serialize(refreshCommand), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/auth/refresh-token", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return false;
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("api/auth/me");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                _logger.LogWarning($"Failed to get current user: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                // Not: AuthService'de forgot-password endpoint'i bulunmuyor.
                // Bu metod şu anda simüle edilmiş bir başarı dönüyor.
                // Gerçek uygulamada, bu endpoint'in eklenmesi veya farklı bir yaklaşım kullanılması gerekebilir.
                _logger.LogWarning("ForgotPasswordAsync called but endpoint does not exist in AuthService");

                // Simüle edilmiş başarı
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                return false;
            }
        }
    }
}
