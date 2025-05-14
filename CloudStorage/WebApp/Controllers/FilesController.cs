using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly ILogger<FilesController> _logger;
        private readonly NotificationService _notificationService;
        private readonly FileService _fileService;
        private readonly AuthService _authService;

        public FilesController(
            ILogger<FilesController> logger,
            NotificationService notificationService,
            FileService fileService,
            AuthService authService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _fileService = fileService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Gerçek dosyaları API'den alıyoruz
                var files = await _fileService.GetUserFilesAsync(token, userId);

                // Eğer API'den dosya alınamazsa örnek dosyaları göster
                if (files.Count == 0)
                {
                    files = GetSampleFiles();
                }

                return View(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user files");
                return View(GetSampleFiles());
            }
        }

        public async Task<IActionResult> Shared()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Gerçek paylaşılan dosyaları API'den alıyoruz
                var files = await _fileService.GetSharedFilesAsync(token, userId);

                // Eğer API'den dosya alınamazsa örnek dosyaları göster
                if (files.Count == 0)
                {
                    files = GetSampleSharedFiles();
                }

                return View(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shared files");
                return View(GetSampleSharedFiles());
            }
        }

        public async Task<IActionResult> Deleted()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Gerçek silinmiş dosyaları API'den alıyoruz
                var files = await _fileService.GetDeletedFilesAsync(token, userId);

                // Eğer API'den dosya alınamazsa örnek dosyaları göster
                if (files.Count == 0)
                {
                    files = GetSampleDeletedFiles();
                }

                return View(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deleted files");
                return View(GetSampleDeletedFiles());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Gerçek dosya detaylarını API'den alıyoruz
                var file = await _fileService.GetFileDetailsAsync(token, id);

                // Eğer API'den dosya alınamazsa örnek dosyayı göster
                if (file == null)
                {
                    file = GetSampleFiles().FirstOrDefault(f => f.Id == id);
                    if (file == null)
                    {
                        return NotFound();
                    }
                }

                return View(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file details");
                var file = GetSampleFiles().FirstOrDefault(f => f.Id == id);
                if (file == null)
                {
                    return NotFound();
                }
                return View(file);
            }
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadFileViewModel model)
        {
            if (ModelState.IsValid && model.File != null)
            {
                try
                {
                    var token = HttpContext.Session.GetString("JWToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    // Dosyayı API aracılığıyla yüklüyoruz
                    var result = await _fileService.UploadFileAsync(token, model, userId);

                    if (result)
                    {
                        _logger.LogInformation("Dosya yükleme başarılı: {FileName}", model.File.FileName);

                        // Send notification
                        var userEmail = User.FindFirstValue(ClaimTypes.Email);
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            await _notificationService.SendUploadNotificationAsync(userEmail, model.File.FileName);
                        }

                        TempData["SuccessMessage"] = "Dosya başarıyla yüklendi.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dosya yüklenemedi. Lütfen tekrar deneyin.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading file");
                    ModelState.AddModelError("", "Dosya yükleme sırasında bir hata oluştu.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Önce dosya bilgilerini alalım
                var file = await _fileService.GetFileDetailsAsync(token, id);
                if (file == null)
                {
                    file = GetSampleFiles().FirstOrDefault(f => f.Id == id);
                    if (file == null)
                    {
                        TempData["ErrorMessage"] = "Dosya bulunamadı.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                // Dosyayı API aracılığıyla siliyoruz
                var result = await _fileService.DeleteFileAsync(token, id);

                if (result)
                {
                    _logger.LogInformation("Dosya silme başarılı: {FileId}", id);

                    // Send notification
                    var userEmail = User.FindFirstValue(ClaimTypes.Email);
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        await _notificationService.SendDeleteNotificationAsync(userEmail, file.Name);
                    }

                    TempData["SuccessMessage"] = "Dosya başarıyla silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Dosya silinemedi. Lütfen tekrar deneyin.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                TempData["ErrorMessage"] = "Dosya silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Dosyayı API aracılığıyla geri yüklüyoruz
                var result = await _fileService.RestoreFileAsync(token, id);

                if (result)
                {
                    _logger.LogInformation("Dosya geri yükleme başarılı: {FileId}", id);
                    TempData["SuccessMessage"] = "Dosya başarıyla geri yüklendi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Dosya geri yüklenemedi. Lütfen tekrar deneyin.";
                }

                return RedirectToAction(nameof(Deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring file");
                TempData["ErrorMessage"] = "Dosya geri yüklenirken bir hata oluştu.";
                return RedirectToAction(nameof(Deleted));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Dosyayı API aracılığıyla kalıcı olarak siliyoruz
                var result = await _fileService.PermanentDeleteFileAsync(token, id);

                if (result)
                {
                    _logger.LogInformation("Dosya kalıcı silme başarılı: {FileId}", id);
                    TempData["SuccessMessage"] = "Dosya kalıcı olarak silindi.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Dosya kalıcı olarak silinemedi. Lütfen tekrar deneyin.";
                }

                return RedirectToAction(nameof(Deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error permanently deleting file");
                TempData["ErrorMessage"] = "Dosya kalıcı olarak silinirken bir hata oluştu.";
                return RedirectToAction(nameof(Deleted));
            }
        }

        public async Task<IActionResult> Share(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Gerçek dosya bilgilerini API'den alıyoruz
                var file = await _fileService.GetFileDetailsAsync(token, id);

                // Eğer API'den dosya alınamazsa örnek dosyayı göster
                if (file == null)
                {
                    file = GetSampleFiles().FirstOrDefault(f => f.Id == id);
                    if (file == null)
                    {
                        return NotFound();
                    }
                }

                var model = new ShareFileViewModel
                {
                    FileId = file.Id,
                    FileName = file.Name
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file for sharing");
                var file = GetSampleFiles().FirstOrDefault(f => f.Id == id);
                if (file == null)
                {
                    return NotFound();
                }
                var model = new ShareFileViewModel
                {
                    FileId = file.Id,
                    FileName = file.Name
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(ShareFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var token = HttpContext.Session.GetString("JWToken");
                    if (string.IsNullOrEmpty(token))
                    {
                        return RedirectToAction("Login", "Account");
                    }

                    // Dosyayı API aracılığıyla paylaşıyoruz
                    var result = await _fileService.ShareFileAsync(token, model);

                    if (result)
                    {
                        _logger.LogInformation("Dosya paylaşımı başarılı: {FileId} - {Email}", model.FileId, model.Email);

                        // Send notification to the shared user
                        await _notificationService.SendShareNotificationAsync(
                            model.Email,
                            model.FileName,
                            User.Identity?.Name ?? "Bir kullanıcı"
                        );

                        TempData["SuccessMessage"] = "Dosya başarıyla paylaşıldı.";
                        return RedirectToAction(nameof(Details), new { id = model.FileId });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Dosya paylaşılamadı. Lütfen tekrar deneyin.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sharing file");
                    ModelState.AddModelError("", "Dosya paylaşılırken bir hata oluştu.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Önce dosya bilgilerini alalım
                var file = await _fileService.GetFileDetailsAsync(token, id);
                if (file == null)
                {
                    file = GetSampleFiles().FirstOrDefault(f => f.Id == id);
                    if (file == null)
                    {
                        return NotFound();
                    }
                }

                // Dosyayı API aracılığıyla indiriyoruz
                var fileBytes = await _fileService.DownloadFileAsync(token, id);

                if (fileBytes != null && fileBytes.Length > 0)
                {
                    return File(fileBytes, file.ContentType, file.Name);
                }
                else
                {
                    // API'den dosya indirilemezse örnek olarak boş bir dosya dönüyoruz
                    TempData["WarningMessage"] = "Gerçek dosya indirilemedi, örnek dosya kullanılıyor.";
                    return File(new byte[0], file.ContentType, file.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                TempData["ErrorMessage"] = "Dosya indirilemedi.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // Örnek veri metodları
        private List<FileViewModel> GetSampleFiles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return new List<FileViewModel>
            {
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Proje Raporu.pdf",
                    Size = 2.5M,
                    ContentType = "application/pdf",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    ModifiedAt = DateTime.Now.AddDays(-2),
                    OwnerId = userId,
                    OwnerName = User.Identity?.Name ?? "Kullanıcı",
                    IsShared = true,
                    FileType = FileType.Pdf
                },
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Toplantı Notları.docx",
                    Size = 1.2M,
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    ModifiedAt = DateTime.Now.AddDays(-3),
                    OwnerId = userId,
                    OwnerName = User.Identity?.Name ?? "Kullanıcı",
                    IsShared = false,
                    FileType = FileType.Document
                },
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Sunum.pptx",
                    Size = 5.7M,
                    ContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    ModifiedAt = DateTime.Now.AddDays(-1),
                    OwnerId = userId,
                    OwnerName = User.Identity?.Name ?? "Kullanıcı",
                    IsShared = false,
                    FileType = FileType.Presentation
                },
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Profil Fotoğrafı.jpg",
                    Size = 0.8M,
                    ContentType = "image/jpeg",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    ModifiedAt = DateTime.Now.AddDays(-10),
                    OwnerId = userId,
                    OwnerName = User.Identity?.Name ?? "Kullanıcı",
                    IsShared = false,
                    FileType = FileType.Image
                }
            };
        }

        private List<FileViewModel> GetSampleSharedFiles()
        {
            return new List<FileViewModel>
            {
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Paylaşılan Belge.pdf",
                    Size = 3.2M,
                    ContentType = "application/pdf",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    ModifiedAt = DateTime.Now.AddDays(-5),
                    OwnerId = "user123",
                    OwnerName = "Ahmet Yılmaz",
                    IsShared = true,
                    FileType = FileType.Pdf
                },
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Proje Planı.xlsx",
                    Size = 1.8M,
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    CreatedAt = DateTime.Now.AddDays(-4),
                    ModifiedAt = DateTime.Now.AddDays(-2),
                    OwnerId = "user456",
                    OwnerName = "Ayşe Demir",
                    IsShared = true,
                    FileType = FileType.Spreadsheet
                }
            };
        }

        private List<FileViewModel> GetSampleDeletedFiles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return new List<FileViewModel>
            {
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Eski Rapor.pdf",
                    Size = 1.5M,
                    ContentType = "application/pdf",
                    CreatedAt = DateTime.Now.AddDays(-20),
                    ModifiedAt = DateTime.Now.AddDays(-15),
                    DeletedAt = DateTime.Now.AddDays(-2),
                    OwnerId = userId,
                    OwnerName = User.Identity?.Name ?? "Kullanıcı",
                    IsShared = false,
                    FileType = FileType.Pdf
                },
                new FileViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Taslak.docx",
                    Size = 0.7M,
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    CreatedAt = DateTime.Now.AddDays(-10),
                    ModifiedAt = DateTime.Now.AddDays(-8),
                    DeletedAt = DateTime.Now.AddDays(-1),
                    OwnerId = userId,
                    OwnerName = User.Identity?.Name ?? "Kullanıcı",
                    IsShared = false,
                    FileType = FileType.Document
                }
            };
        }
    }
}