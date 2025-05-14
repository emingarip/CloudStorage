using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.AddFileVersion;

public class AddFileVersionCommand : IRequest<Result<FileVersionDto>>
{
    public Guid FileId { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
}

public class AddFileVersionCommandHandler : IRequestHandler<AddFileVersionCommand, Result<FileVersionDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AddFileVersionCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileVersionDto>> Handle(AddFileVersionCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var file = await _fileRepository.GetByIdAsync(request.FileId);
        if (file == null)
        {
            return Result.Failure<FileVersionDto>("File not found");
        }

        // Check if user has permission to update the file
        var userId = _currentUserService.UserId;
        if (!await _fileRepository.UserHasAccessToFileAsync(file.Id, userId, FilePermission.Write))
        {
            return Result.Failure<FileVersionDto>("You don't have permission to update this file");
        }

        // Get next version number
        var versionNumber = await _fileRepository.GetNextVersionNumberAsync(file.Id);

        // Add version
        var version = file.AddVersion(request.Path, request.Size, request.ContentType, userId);

        // Save changes
        await _fileRepository.UpdateAsync(file);
        await _fileRepository.SaveChangesAsync();

        // Map to DTO and return
        var versionDto = _mapper.Map<FileVersionDto>(version);
        return Result.Success(versionDto);
    }
}