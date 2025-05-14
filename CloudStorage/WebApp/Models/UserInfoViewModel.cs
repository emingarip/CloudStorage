namespace WebApp.Models
{
    public class UserInfoViewModel
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int TotalFiles { get; set; }
        public decimal TotalStorage { get; set; } // MB cinsinden
        public int SharedFiles { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string GetFormattedStorage()
        {
            if (TotalStorage < 1)
            {
                return $"{TotalStorage * 1024:N0} KB";
            }
            else if (TotalStorage < 1024)
            {
                return $"{TotalStorage:N1} MB";
            }
            else
            {
                return $"{TotalStorage / 1024:N2} GB";
            }
        }
    }
}