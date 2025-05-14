using FileMetadataService.Application.Interfaces;
using FileMetadataService.Domain.Interfaces;
using MediatR;
using SharedKernel;

namespace FileMetadataService.Application.Commands.RemoveFileShare;

public class RemoveFileShareCommand : IRequest<Result>
{
    public Guid FileId { get; set; }
    public Guid ShareId { get; set; }
}

public class RemoveFileShareCommandHandler : IRequestHandler<RemoveFileShareCommand, Result>
{
    private readonly IFileRepository _fileRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFileShareCommandHandler(
        IFileRepository fileRepository,
        ICurrentUserService currentUserService)
    {
        _fileRepository = fileRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RemoveFileShareCommand request, CancellationToken cancellationToken)
    {
        // Get file
        var file = await _fileRepository.GetByIdAsync(request.FileId);
        if (file == null)
        {
            return Result.Failure("File not found");
        }

        // Check if current user is the owner
        var currentUserId = _currentUserService.UserId;
        if (file.OwnerId != currentUserId)
        {
            return Result.Failure("Only the owner can remove shares for this file");
        }

        // Find the share
        var share = file.Shares.FirstOrDefault(s => s.Id == request.ShareId);
        if (share == null)
        {
            return Result.Failure("Share not found");
        }

        // Remove share
        file.RemoveShare(request.ShareId);

        // Save changes
        await _fileRepository.UpdateAsync(file);
        await _fileRepository.SaveChangesAsync();

        return Result.Success();
    }
}