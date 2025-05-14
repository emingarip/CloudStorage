using AutoMapper;
using FileMetadataService.Application.DTOs;
using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Enums;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.ShareFile;

public class ShareFileCommand : IRequest<Result<FileShareDto>>
{
    public Guid FileId { get; set; }
    public Guid UserId { get; set; }
    public FilePermission Permission { get; set; }
}

public class ShareFileCommandHandler : IRequestHandler<ShareFileCommand, Result<FileShareDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ShareFileCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService,
        AutoMapper.IMapper mapper)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<Result<FileShareDto>> Handle(ShareFileCommand request, CancellationToken cancellationToken)
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
            return Result.Failure<FileShareDto>("Only the owner can share this file");
        }

        // Share file
        var share = file.ShareWith(request.UserId, request.Permission);

        // Save changes
        await _fileRepository.UpdateAsync(file);
        await _fileRepository.SaveChangesAsync();

        // Map to DTO and return
        var shareDto = _mapper.Map<FileShareDto>(share);
        return Result.Success(shareDto);
    }
}