using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Domain;
using AutoMapper;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Queries.GetUserByEmail;

public class GetUserByEmailQuery : IRequest<Result<UserDto>>
{
    public string Email { get; set; } = string.Empty;
}

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByEmailQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            return Result.Failure<UserDto>($"User with email {request.Email} not found");
        }

        var userDto = _mapper.Map<UserDto>(user);

        return Result.Success(userDto);
    }
}