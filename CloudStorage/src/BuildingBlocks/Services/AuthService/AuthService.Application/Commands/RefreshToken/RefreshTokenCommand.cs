using System.Threading;
using System.Threading.Tasks;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string IpAddress { get; set; } = "127.0.0.1"; // Default value for simplicity
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate token
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
        {
            return Result.Failure<AuthResponseDto>("Invalid token");
        }

        // Get user id from token
        var userId = _jwtService.GetUserIdFromPrincipal(principal);
        if (userId == null)
        {
            return Result.Failure<AuthResponseDto>("Invalid token");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return Result.Failure<AuthResponseDto>("User not found");
        }

        // Get refresh token
        var refreshToken = await _tokenService.GetRefreshTokenAsync(request.RefreshToken);
        if (refreshToken == null || !refreshToken.IsActive || refreshToken.UserId != userId)
        {
            return Result.Failure<AuthResponseDto>("Invalid refresh token");
        }

        // Revoke current refresh token
        await _tokenService.RevokeRefreshTokenAsync(refreshToken, request.IpAddress, "Refresh token used");

        // Generate new JWT token
        var newToken = _jwtService.GenerateJwtToken(user);

        // Generate new refresh token
        var newRefreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id, request.IpAddress);

        // Return response
        return Result.Success(new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = newToken,
            RefreshToken = newRefreshToken.Token,
            TokenExpiration = _jwtService.GetTokenExpiration()
        });
    }
}