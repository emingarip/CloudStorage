using AuthService.Application.DTOs;
using AuthService.Domain.Interfaces;
using AutoMapper;
using MediatR;
using SharedKernel;

namespace AuthService.Application.Queries.GetUsers;

public class GetUsersQuery : IRequest<Result<List<UserDto>>>
{
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.ListAllAsync();

        var userDtos = _mapper.Map<List<UserDto>>(users);

        return Result.Success(userDtos);
    }
}