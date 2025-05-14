using System.Security.Claims;
using AuthService.Domain;

namespace AuthService.Application.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Guid? GetUserIdFromPrincipal(ClaimsPrincipal principal);
    DateTime GetTokenExpiration();
}