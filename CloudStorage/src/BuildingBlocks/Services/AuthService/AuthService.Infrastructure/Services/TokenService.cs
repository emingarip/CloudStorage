using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Domain;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Identity;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(
        IOptions<JwtSettings> jwtSettings,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public DateTime GetRefreshTokenExpiryTime()
    {
        return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string ipAddress)
    {
        var token = GenerateRefreshToken();
        var expiryTime = GetRefreshTokenExpiryTime();

        var refreshToken = new RefreshToken(token, userId, expiryTime, ipAddress);
        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        return await _refreshTokenRepository.GetByTokenAsync(token);
    }

    public async Task<RefreshToken> GetActiveRefreshTokenByUserIdAsync(Guid userId)
    {
        return await _refreshTokenRepository.GetActiveTokenByUserIdAsync(userId);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, string ipAddress, string reason)
    {
        refreshToken.Revoke(ipAddress, reason);
        await _refreshTokenRepository.UpdateAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();
    }
}