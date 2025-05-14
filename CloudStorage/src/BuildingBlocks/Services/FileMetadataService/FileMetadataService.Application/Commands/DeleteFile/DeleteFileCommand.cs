using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.DeleteFile;

public class DeleteFileCommand : IRequest<Result<FileDto>>
{
    public Guid FileId { get; set; }
}

public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Result<FileDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public DeleteFileCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        AutoMapper.IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileDto>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var fileEntity = await _fileRepository.GetByIdAsync(request.FileId);
        if (fileEntity == null)
        {
            return Result.Failure<FileDto>("File not found");
        }

        // Check if user has permission to delete the file
        var userId = _currentUserService.UserId;
        if (!await _fileRepository.UserHasAccessToFileAsync(fileEntity.Id, userId, FilePermission.Delete))
        {
            return Result.Failure<FileDto>("You don't have permission to delete this file");
        }

        // Delete file
        fileEntity.MarkAsDeleted();
        fileEntity.Status = FileStatus.Deleted;

        // Save changes
        await _fileRepository.UpdateAsync(fileEntity);
        await _fileRepository.SaveChangesAsync();

        // Map to DTO and return
        var fileDto = _mapper.Map<FileDto>(fileEntity);
        return Result.Success(fileDto);
    }
}