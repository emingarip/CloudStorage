using AuthService.Domain;

namespace AuthService.Application.Interfaces;

public interface ITokenService
{
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpiryTime();
    Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string ipAddress);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    Task<RefreshToken> GetActiveRefreshTokenByUserIdAsync(Guid userId);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string ipAddress, string reason);
}