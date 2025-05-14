using System;
using System.Security.Cryptography;
using System.Text;
using AuthService.Application.Interfaces;

namespace AuthService.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private const int KeySize = 64;
        private const int Iterations = 10000;
        private readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

        public (string passwordHash, string salt) HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(KeySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                _hashAlgorithm,
                KeySize);

            return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var salt = Convert.FromBase64String(storedSalt);
            var hash = Convert.FromBase64String(storedHash);

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                _hashAlgorithm,
                KeySize);

            return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
        }
    }
}