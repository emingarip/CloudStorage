using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.RestoreFile;

public class RestoreFileCommand : IRequest<Result<FileDto>>
{
    public Guid FileId { get; set; }
}

public class RestoreFileCommandHandler : IRequestHandler<RestoreFileCommand, Result<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public RestoreFileCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileDto>> Handle(RestoreFileCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var file = await _fileRepository.GetByIdAsync(request.FileId);
        if (file == null)
        {
            return Result.Failure<FileDto>("File not found");
        }

        // Check if current user is the owner
        var currentUserId = _currentUserService.UserId;
        if (file.OwnerId != currentUserId)
        {
            return Result.Failure<FileDto>("Only the owner can restore this file");
        }

        // Restore file
        file.Restore();

        // Save changes
        await _fileRepository.UpdateAsync(file);
        await _fileRepository.SaveChangesAsync();

        // Map to DTO and return
        var fileDto = _mapper.Map<FileDto>(file);
        return Result.Success(fileDto);
    }
}