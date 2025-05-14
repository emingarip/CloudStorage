using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Queries.GetSharedFiles;

public class GetSharedFilesQuery : IRequest<Result<List<FileDto>>>
{
}

public class GetSharedFilesQueryHandler : IRequestHandler<GetSharedFilesQuery, Result<List<FileDto>>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetSharedFilesQueryHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<List<FileDto>>> Handle(GetSharedFilesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // Get files shared with the user
        var files = await _fileRepository.GetSharedFilesWithUserAsync(userId);

        // Map to DTOs and return
        var fileDtos = _mapper.Map<List<FileDto>>(files);
        return Result.Success(fileDtos);
    }
}