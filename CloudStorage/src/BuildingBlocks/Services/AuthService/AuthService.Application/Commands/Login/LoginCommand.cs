using System.Threading;
using System.Threading.Tasks;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Commands.Login;

public class LoginCommand : IRequest<Result<AuthResponseDto>>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; } = "127.0.0.1"; // Default value for simplicity
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get user by username
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            return Result.Failure<AuthResponseDto>("Invalid username or password");
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            return Result.Failure<AuthResponseDto>("Invalid username or password");
        }

        // Check if user has an active refresh token and revoke it
        var activeRefreshToken = await _tokenService.GetActiveRefreshTokenByUserIdAsync(user.Id);
        if (activeRefreshToken != null)
        {
            await _tokenService.RevokeRefreshTokenAsync(activeRefreshToken, request.IpAddress, "New login");
        }

        // Generate JWT token
        var token = _jwtService.GenerateJwtToken(user);

        // Generate new refresh token
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id, request.IpAddress);

        // Return response
        return Result.Success(new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = token,
            RefreshToken = refreshToken.Token,
            TokenExpiration = _jwtService.GetTokenExpiration()
        });
    }
}