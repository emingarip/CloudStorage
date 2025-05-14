using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AuthService.Domain;
using AutoMapper;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            return Result.Failure<UserDto>("User not found");
        }

        var userDto = _mapper.Map<UserDto>(user);

        return Result.Success(userDto);
    }
}