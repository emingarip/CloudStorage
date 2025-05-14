using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Queries.GetUserFiles;

public class GetUserFilesQuery : IRequest<Result<List<FileDto>>>
{
    public Guid? UserId { get; set; }
}

public class GetUserFilesQueryHandler : IRequestHandler<GetUserFilesQuery, Result<List<FileDto>>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetUserFilesQueryHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<List<FileDto>>> Handle(GetUserFilesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var targetUserId = request.UserId ?? currentUserId;

        // Users can only view their own files
        if (targetUserId != currentUserId && !_currentUserService.IsAdmin)
        {
            return Result.Failure<List<FileDto>>("You can only view your own files");
        }

        // Get files
        var files = await _fileRepository.GetFilesByOwnerIdAsync(targetUserId);

        // Map to DTOs and return
        var fileDtos = _mapper.Map<List<FileDto>>(files);
        return Result.Success(fileDtos);
    }
}