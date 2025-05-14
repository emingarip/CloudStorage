using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public enum FileType
    {
        Document,
        Spreadsheet,
        Presentation,
        Pdf,
        Image,
        Video,
        Audio,
        Archive,
        Other
    }

    public class FileViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Size { get; set; } // MB cinsinden
        public string ContentType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public bool IsShared { get; set; }
        public FileType FileType { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? DownloadUrl { get; set; }
        public List<FileShareViewModel> Shares { get; set; } = new List<FileShareViewModel>();

        public string GetFileIcon()
        {
            return FileType switch
            {
                FileType.Document => "bi-file-earmark-word",
                FileType.Spreadsheet => "bi-file-earmark-excel",
                FileType.Presentation => "bi-file-earmark-ppt",
                FileType.Pdf => "bi-file-earmark-pdf",
                FileType.Image => "bi-file-earmark-image",
                FileType.Video => "bi-file-earmark-play",
                FileType.Audio => "bi-file-earmark-music",
                FileType.Archive => "bi-file-earmark-zip",
                _ => "bi-file-earmark"
            };
        }

        public string GetFileIconColor()
        {
            return FileType switch
            {
                FileType.Document => "text-primary",
                FileType.Spreadsheet => "text-success",
                FileType.Presentation => "text-warning",
                FileType.Pdf => "text-danger",
                FileType.Image => "text-info",
                FileType.Video => "text-danger",
                FileType.Audio => "text-primary",
                FileType.Archive => "text-warning",
                _ => "text-secondary"
            };
        }

        public string GetFormattedSize()
        {
            if (Size < 1)
            {
                return $"{Size * 1024:N0} KB";
            }
            return $"{Size:N1} MB";
        }
    }

    public class FileShareViewModel
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Permission { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UploadFileViewModel
    {
        [Required(ErrorMessage = "Lütfen bir dosya seçin")]
        [Display(Name = "Dosya")]
        public IFormFile? File { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Etiketler")]
        public string? Tags { get; set; }
    }

    public enum FilePermissionType
    {
        Read = 1,
        Write = 2,
        Delete = 3
    }

    public class ShareFileViewModel
    {
        public Guid FileId { get; set; }

        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        // Kullanıcı ID'si (API tarafından kullanılacak)
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "İzin türü gereklidir")]
        [Display(Name = "İzin")]
        public string PermissionString { get; set; } = "read";

        // API'ye gönderilecek enum değeri
        public FilePermissionType Permission =>
            PermissionString == "edit" || PermissionString == "write" ?
            FilePermissionType.Write :
            FilePermissionType.Read;

        [Display(Name = "Not")]
        public string? Note { get; set; }
    }
}