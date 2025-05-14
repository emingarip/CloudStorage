using System.Threading;
using System.Threading.Tasks;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain;
using AuthService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Commands.Register;

public class RegisterCommand : IRequest<Result<AuthResponseDto>>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; } = "127.0.0.1"; // Default value for simplicity
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(
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

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return Result.Failure<AuthResponseDto>("Username already exists");
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return Result.Failure<AuthResponseDto>("Email already exists");
        }

        // Hash password
        var (passwordHash, salt) = _passwordService.HashPassword(request.Password);

        // Create user
        var user = new User(request.Username, request.Email, passwordHash, salt);

        // Save user
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        // Generate JWT token
        var token = _jwtService.GenerateJwtToken(user);

        // Generate refresh token
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