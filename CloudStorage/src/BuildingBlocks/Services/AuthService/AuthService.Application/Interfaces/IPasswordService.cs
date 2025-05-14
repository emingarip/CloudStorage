namespace AuthService.Application.Interfaces;

public interface IPasswordService
{
    (string passwordHash, string salt) HashPassword(string password);
    bool VerifyPassword(string password, string storedHash, string storedSalt);
}