namespace AuthService.Infrastructure.Identity;

public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int DurationInMinutes { get; set; }
    public int RefreshTokenDurationInDays { get; set; }
}