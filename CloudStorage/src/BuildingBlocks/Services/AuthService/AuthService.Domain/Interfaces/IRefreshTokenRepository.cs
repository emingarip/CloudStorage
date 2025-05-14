namespace AuthService.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<RefreshToken> GetActiveTokenByUserIdAsync(Guid userId);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task SaveChangesAsync();
}