using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Queries.GetDeletedFiles;

public class GetDeletedFilesQuery : IRequest<Result<List<FileDto>>>
{
}

public class GetDeletedFilesQueryHandler : IRequestHandler<GetDeletedFilesQuery, Result<List<FileDto>>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetDeletedFilesQueryHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<List<FileDto>>> Handle(GetDeletedFilesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        // Get deleted files
        var files = await _fileRepository.GetDeletedFilesByOwnerIdAsync(userId);

        // Map to DTOs and return
        var fileDtos = _mapper.Map<List<FileDto>>(files);
        return Result.Success(fileDtos);
    }
}