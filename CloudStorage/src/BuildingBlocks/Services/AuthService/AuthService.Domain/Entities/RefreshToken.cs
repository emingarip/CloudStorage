using System;
using SharedKernel;

namespace AuthService.Domain;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public DateTime CreatedAt { get; private set; }
    public string CreatedByIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string RevokedByIp { get; private set; }
    public string ReasonRevoked { get; private set; }
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    // For EF Core
    private RefreshToken() { }

    public RefreshToken(string token, Guid userId, DateTime expiryDate, string createdByIp)
    {
        Token = token;
        UserId = userId;
        ExpiryDate = expiryDate;
        CreatedAt = DateTime.UtcNow;
        CreatedByIp = createdByIp;
    }

    public void Revoke(string revokedByIp, string reason = null)
    {
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReasonRevoked = reason;
    }
}