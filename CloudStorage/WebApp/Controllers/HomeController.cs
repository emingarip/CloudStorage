using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthService _authService;

        public HomeController(
            ILogger<HomeController> logger,
            AuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kullanıcı bilgilerini API'den al
                var currentUser = await _authService.GetCurrentUserAsync(token);

                if (currentUser != null)
                {
                    var userInfo = new UserInfoViewModel
                    {
                        UserId = currentUser.Id,
                        Username = currentUser.Username,
                        Email = currentUser.Email,
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName
                    };

                    // Dosya istatistikleri için örnek veriler
                    userInfo.TotalFiles = 12;
                    userInfo.TotalStorage = 250;
                    userInfo.SharedFiles = 3;

                    return View(userInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
            }

            // Fallback to claims-based info if API call fails
            var userInfoFromClaims = new UserInfoViewModel
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = User.Identity?.Name,
                Email = User.FindFirst(ClaimTypes.Email)?.Value
            };

            return View(userInfoFromClaims);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
