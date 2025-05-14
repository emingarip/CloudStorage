using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;
using WebApp.Extensions;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);

                if (result != null)
                {
                    // Kullanıcı kimlik bilgilerini oluştur
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                        new Claim(ClaimTypes.Name, result.Username ?? string.Empty),
                        new Claim(ClaimTypes.Email, result.Email ?? string.Empty)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    // Kullanıcıyı sisteme giriş yaptır
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Token'ı session'a kaydet
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "JWToken", result.Token ?? string.Empty);
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "RefreshToken", result.RefreshToken ?? string.Empty);

                    _logger.LogInformation("Kullanıcı başarıyla giriş yaptı: {Username}", model.Username);

                    // Yönlendirme URL'i varsa oraya, yoksa ana sayfaya yönlendir
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi. Kullanıcı adı veya şifre hatalı.");
                _logger.LogWarning("Başarısız giriş denemesi: {Username}", model.Username);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model);

                if (result != null)
                {
                    _logger.LogInformation("Kullanıcı başarıyla kaydedildi: {Email}", model.Email);

                    // Kullanıcı kimlik bilgilerini oluştur
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                        new Claim(ClaimTypes.Name, result.Username ?? string.Empty),
                        new Claim(ClaimTypes.Email, result.Email ?? string.Empty)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    // Kullanıcıyı sisteme giriş yaptır
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Token'ı session'a kaydet
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "JWToken", result.Token ?? string.Empty);
                    Microsoft.AspNetCore.Http.SessionExtensions.SetString(HttpContext.Session, "RefreshToken", result.RefreshToken ?? string.Empty);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Kayıt işlemi başarısız oldu. Lütfen tekrar deneyin.");
                _logger.LogWarning("Başarısız kayıt denemesi: {Email}", model.Email);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ForgotPasswordAsync(model.Email);

                if (result)
                {
                    _logger.LogInformation("Şifre sıfırlama e-postası gönderildi: {Email}", model.Email);
                    ViewBag.Message = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
                    return View();
                }

                ModelState.AddModelError(string.Empty, "Şifre sıfırlama işlemi başarısız oldu. Lütfen tekrar deneyin.");
                _logger.LogWarning("Şifre sıfırlama başarısız: {Email}", model.Email);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Kullanıcıyı sistemden çıkış yaptır
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Session'ı temizle
            HttpContext.Session.Clear();

            _logger.LogInformation("Kullanıcı çıkış yaptı");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}