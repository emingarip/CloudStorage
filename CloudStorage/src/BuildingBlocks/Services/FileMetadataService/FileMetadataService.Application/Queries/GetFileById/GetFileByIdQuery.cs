using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Queries.GetFileById;

public class GetFileByIdQuery : IRequest<Result<FileDto>>
{
    public Guid FileId { get; set; }
}

public class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, Result<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetFileByIdQueryHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileDto>> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        // Get file
        var file = await _fileRepository.GetByIdAsync(request.FileId);
        if (file == null)
        {
            return Result.Failure<FileDto>("File not found");
        }

        // Check if user has permission to read the file
        var userId = _currentUserService.UserId;
        if (!await _fileRepository.UserHasAccessToFileAsync(file.Id, userId, FilePermission.Read))
        {
            return Result.Failure<FileDto>("You don't have permission to access this file");
        }

        // Map to DTO and return
        var fileDto = _mapper.Map<FileDto>(file);
        return Result.Success(fileDto);
    }
}