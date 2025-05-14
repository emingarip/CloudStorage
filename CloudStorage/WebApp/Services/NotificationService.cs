namespace WebApp.Services
{
    public class NotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailNotificationAsync(string email, string subject, string message)
        {
            // Bu metod gerçek uygulamada e-posta gönderimi yapacak
            // Şimdilik sadece log kaydı oluşturuyoruz
            _logger.LogInformation($"E-posta gönderildi: {email}, Konu: {subject}");
            await Task.CompletedTask;
        }

        public async Task SendShareNotificationAsync(string email, string fileName, string sharedByUsername)
        {
            string subject = $"{sharedByUsername} bir dosya paylaştı";
            string message = $"{sharedByUsername} kullanıcısı '{fileName}' dosyasını sizinle paylaştı. Dosyaya erişmek için hesabınıza giriş yapabilirsiniz.";

            await SendEmailNotificationAsync(email, subject, message);
        }

        public async Task SendUploadNotificationAsync(string email, string fileName)
        {
            string subject = "Dosya yükleme başarılı";
            string message = $"'{fileName}' dosyası başarıyla yüklendi. Dosyaya hesabınızdan erişebilirsiniz.";

            await SendEmailNotificationAsync(email, subject, message);
        }

        public async Task SendDeleteNotificationAsync(string email, string fileName)
        {
            string subject = "Dosya silindi";
            string message = $"'{fileName}' dosyası silindi. Dosya 30 gün boyunca çöp kutusunda kalacak ve bu süre sonunda otomatik olarak kalıcı olarak silinecek.";

            await SendEmailNotificationAsync(email, subject, message);
        }
    }
}