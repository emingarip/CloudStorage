using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.UpdateFileShare;

public class UpdateFileShareCommand : IRequest<Result<FileShareDto>>
{
    public Guid FileId { get; set; }
    public Guid ShareId { get; set; }
    public FilePermission Permission { get; set; }
}

public class UpdateFileShareCommandHandler : IRequestHandler<UpdateFileShareCommand, Result<FileShareDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateFileShareCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileShareDto>> Handle(UpdateFileShareCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var file = await _fileRepository.GetByIdAsync(request.FileId);
        if (file == null)
        {
            return Result.Failure<FileShareDto>("File not found");
        }

        // Check if current user is the owner
        var currentUserId = _currentUserService.UserId;
        if (file.OwnerId != currentUserId)
        {
            return Result.Failure<FileShareDto>("Only the owner can update shares for this file");
        }

        // Find the share
        var share = file.Shares.FirstOrDefault(s => s.Id == request.ShareId);
        if (share == null)
        {
            return Result.Failure<FileShareDto>("Share not found");
        }

        // Update share
        file.UpdateShare(request.ShareId, request.Permission);

        // Save changes
        await _fileRepository.UpdateAsync(file);
        await _fileRepository.SaveChangesAsync();

        // Map to DTO and return
        var updatedShare = file.Shares.First(s => s.Id == request.ShareId);
        var shareDto = _mapper.Map<FileShareDto>(updatedShare);
        return Result.Success(shareDto);
    }
}